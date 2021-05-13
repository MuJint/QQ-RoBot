using Microsoft.Extensions.DependencyInjection;
using Qiushui.Framework.Interface;
using Qiushui.Framework.Services;
using Sora.Server;
using Sora.Tool;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Qiushui.Bot
{
    static class BotServerInterface
    {
        static async Task Main()
        {
            //IOC
            Dependcy.Provider = new ServiceCollection()
                .AddScoped<ILianChatServices, LianChatServices>()
                .AddScoped<ILianKeyWordsServices, LianKeyWordsServices>()
                .AddScoped<ISignLogsServices, SignLogsServices>()
                .AddScoped<ISignUserServices, SignUserServices>()
                .AddScoped<ISpeakerServices, SpeakerServices>()
                .AddScoped<IRebortInterface, RebortService>()
                .AddScoped<ILianInterface, LianService>()
                .AddScoped<IModuleInterface, ModuleService>()
                .BuildServiceProvider();
            var Instance = Dependcy.Provider.GetService<IRebortInterface>();

            //修改控制台标题
            Console.Title = "Qiushui.Bot";
            ConsoleLog.Info("Qiushui.Bot初始化", "Qiushui.Bot初始化...");
            //初始化配置文件
            ConsoleLog.Info("Qiushui.Bot初始化", "初始化服务器全局配置...");
            Config config = new(0);
            config.GlobalConfigFileInit();
            config.LoadGlobalConfig(out GlobalConfig globalConfig, false);

            ConsoleLog.SetLogLevel(Fleck.LogLevel.Info);
            //显示Log等级
            ConsoleLog.Debug("Log Level", globalConfig.LogLevel);

            //初始化字符编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //指令匹配初始化
            CommandHelper.KeywordResourseInit();

            //
            ConsoleLog.Info("Qiushui.Bot初始化", "初始化指令匹配集...");
            ConsoleLog.Info("Qiushui.Bot初始化", "启动反向WebSocket服务器...");
            //初始化服务器
            SoraWSServer server = new(new ServerConfig
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

            server.Event.OnClientConnect += Instance.Initalization;
            server.Event.OnGroupMessage += Instance.GroupMessageParse;
            server.Event.OnPrivateMessage += Instance.PrivateMessageParse;
            server.Event.OnGroupPoke += Instance.GroupPokeEventParse;
            server.Event.OnFriendAdd += Instance.FriendAddParse;
            server.Event.OnGroupMemberChange += Instance.GroupMemberChangeParse;
            server.Event.OnGroupRecall += Instance.GroupRecallParse;
            //更多事件按需处理

            //关闭连接事件处理
            //server.ConnManager.OnCloseConnectionAsync += TimerEventParse.StopTimer;
            //server.ConnManager.OnHeartBeatTimeOut += TimerEventParse.StopTimer;

            await server.StartServer();
        }
    }
}
