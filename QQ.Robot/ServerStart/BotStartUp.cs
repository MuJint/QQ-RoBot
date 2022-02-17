using Microsoft.Extensions.DependencyInjection;
using Robot.Common.Helper;
using Robot.Common.Interface;
using Robot.Common.Enums;
using Robot.Framework.Interface;
using Robot.Framework.Services;
using Sora;
using Sora.Interfaces;
using Sora.Net.Config;
using System;
using System.Text;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    static class BotStartUp
    {
        static async Task Main()
        {
            //IOC
            Dependcy.Provider = new ServiceCollection()
                .AddScoped<ILogsInterface, LogsHelper>()
                .AddScoped<ILianChatServices, LianChatServices>()
                .AddScoped<ILianKeyWordsServices, LianKeyWordsServices>()
                .AddScoped<ISignLogsServices, SignLogsServices>()
                .AddScoped<ISignUserServices, SignUserServices>()
                .AddScoped<ISpeakerServices, SpeakerServices>()
                .AddScoped<IRobotInterface, RobotService>()
                .AddScoped<ILianInterface, LianService>()
                .AddScoped<IHsoInterface, HsoService>()
                .BuildServiceProvider();
            var Instance = Dependcy.Provider.GetService<IRobotInterface>();
            var _logs = Dependcy.Provider.GetService<ILogsInterface>();

            //修改控制台标题
            Console.Title = "Bot";
            _logs.Info("Bot初始化", "Bot初始化...");
            //初始化配置文件
            _logs.Info("Bot初始化", "初始化服务器全局配置...");
            Config config = new(0);
            config.GlobalConfigFileInit();
            config.LoadGlobalConfig(out GlobalConfig globalConfig, false);

            _logs.SetLogLevel(EnumLogLevel.Info);
            //显示_logs等级
            _logs.Info("LogLevel", $"{globalConfig.LogLevel}");

            //初始化字符编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //指令匹配初始化
            CommandHelper.KeywordResourseInit();

            //初始化接口对应服务字典
            GlobalSettings.GlobalSettingsInit();

            //
            _logs.Info("Bot初始化", "初始化指令匹配集");
            _logs.Info("Bot初始化", "启动反向WebSocket服务器");
            //初始化服务器
            ISoraService server = SoraServiceFactory.CreateService(new ServerConfig
            {
                Host = globalConfig.Location,
                Port = globalConfig.Port,
                AccessToken = globalConfig.AccessToken,
                UniversalPath = globalConfig.UniversalPath,
                HeartBeatTimeOut = TimeSpan.FromSeconds(globalConfig.HeartBeatTimeOut),
                ApiTimeOut = TimeSpan.FromSeconds(globalConfig.ApiTimeOut),
                EnableSoraCommandManager = true
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

            await server.StartService();
            await Task.Delay(-1);
        }
    }
}
