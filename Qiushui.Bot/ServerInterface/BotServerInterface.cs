using Qiushui.Bot.Helper.ConfigModule;
using Qiushui.Bot.Resource;
using Sora.Server;
using Sora.Tool;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Qiushui.Bot.ServerInterface
{
    static class BotServerInterface
    {
        static async Task Main()
        {
            try
            {
                //修改控制台标题
                Console.Title = @"Qiushui.Bot";
                ConsoleLog.Info("Qiushui.Bot初始化", "Qiushui.Bot初始化...");
                //初始化配置文件
                ConsoleLog.Info("Qiushui.Bot初始化", "初始化服务器全局配置...");
                //全局文件初始化不需要uid，填0仅占位，不使用构造函数重载
                Config config = new Config(0);
                config.GlobalConfigFileInit();
                config.LoadGlobalConfig(out GlobalConfig globalConfig, false);


                ConsoleLog.SetLogLevel(Fleck.LogLevel.Info);
                //显示Log等级
                ConsoleLog.Debug("Log Level", globalConfig.LogLevel);

                //初始化字符编码
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                //指令匹配初始化
                Command.KeywordResourseInit();
                Command.RegexResourseInit();
                //
                ConsoleLog.Info("Qiushui.Bot初始化", "初始化指令匹配集...");
                ConsoleLog.Info("Qiushui.Bot初始化", "启动反向WebSocket服务器...");
                //初始化服务器
                SoraWSServer server = new SoraWSServer(new ServerConfig
                {
                    Location = globalConfig.Location,
                    Port = globalConfig.Port,
                    AccessToken = globalConfig.AccessToken,
                    UniversalPath = globalConfig.UniversalPath,
                    ApiPath = globalConfig.ApiPath,
                    EventPath = globalConfig.EventPath,
                    HeartBeatTimeOut = globalConfig.HeartBeatTimeOut,
                    ApiTimeOut = globalConfig.ApiTimeOut
                });

                //服务器回调
                //初始化
                server.Event.OnClientConnect += InitalizationEvent.Initalization;
                //群聊事件
                server.Event.OnGroupMessage += GroupMessageEvent.GroupMessageParse;
                //私聊事件
                server.Event.OnPrivateMessage += PrivateMessageEvent.PrivateMessageParse;
                //群聊戳一戳
                server.Event.OnGroupPoke += GroupPokeEvent.GroupPokeEventParse;
                //好友申请事件
                server.Event.OnFriendAdd += FriendAddEvent.FriendAddParse;
                //群成员变更事件
                server.Event.OnGroupMemberChange += GroupMemberChangeEvent.GroupMemberChangeParse;
                //群撤回消息事件
                server.Event.OnGroupRecall += GroupRecallEvent.GroupRecallParse;
                //更多事件按需处理

                //关闭连接事件处理
                //server.ConnManager.OnCloseConnectionAsync += TimerEventParse.StopTimer;
                //server.ConnManager.OnHeartBeatTimeOut += TimerEventParse.StopTimer;

                await server.StartServer();
            }
            catch (Exception c)
            {
                ConsoleLog.Error("error", c);
            }
        }
    }
}
