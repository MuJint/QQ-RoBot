using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Robot.Common.Enums;
using Robot.Common.Interface;
using Sora;
using Sora.Interfaces;
using Sora.Net.Config;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    /// <summary>
    /// 工作类
    /// </summary>
    public class IWorker : IHostedService
    {
        private readonly ILogsInterface _logs;
        private readonly IRobotInterface _robot;
        private readonly IHostLifetime _lifetime;
        private readonly IConfiguration _configuration;

        public IWorker(IHostLifetime lifetime, 
            ILogsInterface logs, 
            IRobotInterface robot, 
            IConfiguration configuration)
        {
            _lifetime = lifetime;
            _logs = logs;
            _robot = robot;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logs.Info("START","start applictin");
            _logs.Info("START","start websocket");
            await StartWebSocket();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logs.Info("EXIT", "exit appliction");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 启动工作socket
        /// </summary>
        /// <returns></returns>
        private async Task StartWebSocket()
        {
            //修改控制台标题
            Console.Title = "Bot";
            _logs.Info("Bot初始化", "Bot初始化...");
            //初始化配置文件
            _logs.Info("Bot初始化", "初始化服务器全局配置...");

            _logs.SetLogLevel(EnumLogLevel.Information);

            //初始化字符编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //指令匹配初始化
            CommandHelper.KeywordResourseInit();

            //初始化接口对应服务字典
            GlobalSettings.InitalizeInterfaceToService();

            _logs.Info("Bot初始化", "启动反向WebSocket服务器");
            //初始化服务器
            ISoraService server = SoraServiceFactory.CreateService(new ServerConfig
            {
                Host = GlobalSettings.AppSetting.ServerConfig.Location,
                Port = GlobalSettings.AppSetting.ServerConfig.Port,
                AccessToken = GlobalSettings.AppSetting.ServerConfig.AccessToken,
                UniversalPath = GlobalSettings.AppSetting.ServerConfig.UniversalPath,
                HeartBeatTimeOut = TimeSpan.FromSeconds(GlobalSettings.AppSetting.ServerConfig.HeartBeatTimeOut),
                ApiTimeOut = TimeSpan.FromSeconds(GlobalSettings.AppSetting.ServerConfig.ApiTimeOut),
                EnableSoraCommandManager = true
            });

            server.Event.OnClientConnect += _robot.Initalization;
            server.Event.OnGroupMessage += _robot.GroupMessageParse;
            server.Event.OnPrivateMessage += _robot.PrivateMessageParse;
            server.Event.OnGroupPoke += _robot.GroupPokeEventParse;
            server.Event.OnFriendAdd += _robot.FriendAddParse;
            server.Event.OnGroupMemberChange += _robot.GroupMemberChangeParse;
            server.Event.OnGroupRecall += _robot.GroupRecallParse;
            //更多事件按需处理

            //关闭连接事件处理
            //server.ConnManager.OnCloseConnectionAsync += TimerEventParse.StopTimer;
            //server.ConnManager.OnHeartBeatTimeOut += TimerEventParse.StopTimer;

            await server.StartService();
            await Task.Delay(-1);
        }
    }
}
