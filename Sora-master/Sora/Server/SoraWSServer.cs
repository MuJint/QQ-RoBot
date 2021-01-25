using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Fleck;
using Newtonsoft.Json.Linq;
using Sora.Exceptions;
using Sora.Server.ServerInterface;
using Sora.Tool;

namespace Sora.Server
{
    /// <summary>
    /// Sora服务器实例
    /// </summary>
    public sealed class SoraWSServer : IDisposable
    {
        #region 属性
        /// <summary>
        /// 服务器配置类
        /// </summary>
        private ServerConfig Config { get; set; }

        /// <summary>
        /// WS服务器
        /// </summary>
        private WebSocketServer Server { get; set; }

        /// <summary>
        /// 心跳包检查计时器
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private Timer HeartBeatTimer { get; set; }

        /// <summary>
        /// 事件接口
        /// </summary>
        public EventInterface Event { get; set; }

        /// <summary>
        /// 服务器连接管理器
        /// </summary>
        public ConnectionManager ConnManager { get; set; }
        #endregion

        #region 私有字段
        /// <summary>
        /// 服务器已准备启动标识
        /// </summary>
        private readonly bool serverReady;
        /// <summary>
        /// 当前进程服务器已存在的标识
        /// </summary>
        private static bool serverExitis;
        #endregion

        #region 构造函数
        /// <summary>
        /// 静态构造函数
        /// 用于初始化静态处理资源
        /// </summary>
        static SoraWSServer()
        {
            //全局异常事件
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                                                          {
                                                              ConsoleLog.UnhandledExceptionLog(args);
                                                          };
        }

        /// <summary>
        /// 创建一个反向WS服务端
        /// </summary>
        /// <param name="config">服务器配置</param>
        public SoraWSServer(ServerConfig config)
        {
            //检查端口占用
            if (PortInUse(config.Port))
            {
                ConsoleLog.Fatal("Sora", $"端口{config.Port}已被占用，请更换其他端口");
                ConsoleLog.Warning("Sora", "将在5s后自动退出");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
            serverReady = false;
            ConsoleLog.Info("Sora",$"Sora WebSocket服务器初始化...");
            ConsoleLog.Debug("System",Environment.OSVersion);
            //初始化连接管理器
            ConnManager = new ConnectionManager(config);
            //检查参数
            if(config == null) throw new ArgumentNullException(nameof(config));
            if (config.Port == 0 || config.Port > 65535) throw new ArgumentOutOfRangeException(nameof(config.Port));
            this.Config = config;
            //API超时
            ApiInterface.TimeOut = config.ApiTimeOut;
            //实例化事件接口
            this.Event = new EventInterface();
            //禁用原log
            FleckLog.Level = (LogLevel)4;
            this.Server    = new WebSocketServer($"ws://{config.Location}:{config.Port}")
            {
                //出错后进行重启
                RestartAfterListenError = true
            };
            serverReady = true;
        }
        #endregion

        #region 服务端启动
        /// <summary>
        /// 启动WS服务端
        /// </summary>
        public async ValueTask StartServer()
        {
            if(!serverReady) return;
            //检查是否已有服务器被启动
            if(serverExitis) throw new SoraServerIsRuningException();
            //心跳包超时检查计时器
            this.HeartBeatTimer = new Timer(ConnManager.HeartBeatCheck, null, new TimeSpan(0, 0, 0, (int)Config.HeartBeatTimeOut, 0),
                                            new TimeSpan(0, 0, 0, (int)Config.HeartBeatTimeOut, 0));
            Server.Start(socket =>
                         {
                             //接收事件处理
                             //获取请求头数据
                             if (!socket.ConnectionInfo.Headers.TryGetValue("X-Self-ID",
                                                                            out string selfId) ||       //bot UID
                                 !socket.ConnectionInfo.Headers.TryGetValue("X-Client-Role",
                                                                            out string role))           //Client Type
                             {return;}

                             var isLost = role switch
                             {
                                 "Universal" => !socket.ConnectionInfo.Path.Trim('/').Equals(Config.UniversalPath),
                                 "Event" => !socket.ConnectionInfo.Path.Trim('/').Equals(Config.EventPath),
                                 "API" => !socket.ConnectionInfo.Path.Trim('/').Equals(Config.ApiPath),
                                 _ => false,
                             };
                             if (isLost)
                             {
                                 socket.Close();
                                 ConsoleLog.Warning("Sora",
                                                    $"关闭与未知客户端的连接[{socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}]，请检查是否设置正确的监听地址");
                                 return;
                             }
                             //打开连接
                             socket.OnOpen = () =>
                                             {
                                                 //获取Token
                                                 if (socket.ConnectionInfo.Headers.TryGetValue("Authorization",out string token))
                                                 {
                                                     //验证Token
                                                     if(!token.Equals(this.Config.AccessToken)) return;
                                                 }
                                                 //向客户端发送Ping
                                                 socket.SendPing(new byte[] { 1, 2, 5 });
                                                 //事件回调
                                                 ConnManager.OpenConnection(role, selfId, socket);
                                                 ConsoleLog.Info("Sora",
                                                                 $"已连接客户端[{socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}]");
                                             };
                             //关闭连接
                             socket.OnClose = () =>
                                              {
                                                  //移除原连接信息
                                                  if (ConnManager.ConnectionExitis(socket.ConnectionInfo.Id))
                                                      ConnManager.CloseConnection(role, selfId, socket);

                                                  ConsoleLog.Info("Sora",
                                                                  $"客户端连接被关闭[{socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}]");
                                              };
                             //上报接收
                             socket.OnMessage = (message) =>
                                                {
                                                    //处理接收的数据
                                                    // ReSharper disable once SimplifyLinqExpressionUseAll
                                                    if (!ConnManager.ConnectionExitis(socket.ConnectionInfo.Id)) return;
                                                    //进入事件处理和分发
                                                    Task.Run(() =>
                                                             {
                                                                 this.Event
                                                                     .Adapter(JObject.Parse(message),
                                                                              socket.ConnectionInfo.Id);
                                                             });
                                                };
                         });
            ConsoleLog.Info("Sora",$"Sora WebSocket服务器正在运行[{Config.Location}:{Config.Port}]");
            ConsoleLog.Info("Sora",$"Sora 服务端框架版本:{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");

            serverExitis = true;
            await Task.Delay(-1);
        }
        /// <summary>
        /// GC析构函数
        /// </summary>
        ~SoraWSServer()
        {
            Dispose();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Server?.Dispose();
            ApiInterface.RequestList.Clear();
        }
        #endregion

        #region 服务器事件处理方法
        /// <summary>
        /// 检查端口占用
        /// </summary>
        /// <param name="port">端口号</param>
        private static bool PortInUse(uint port) =>
            IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
                              .Any(ipEndPoint => ipEndPoint.Port == port);
        #endregion
    }
}
