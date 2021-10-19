﻿using Microsoft.Extensions.DependencyInjection;
using Robot.Framework.Interface;
using Robot.Framework.Services;
using Sora.Interfaces;
using Sora.Net;
using Sora.Net.Config;
using System;
using System.Text;
using System.Threading.Tasks;
using YukariToolBox.FormatLog;

namespace QQ.RoBot
{
    static class BotStartUp
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
                .AddScoped<IRobotInterface, RobotService>()
                .AddScoped<ILianInterface, LianService>()
                .AddScoped<IHsoInterface, HsoService>()
                .BuildServiceProvider();
            var Instance = Dependcy.Provider.GetService<IRobotInterface>();

            //修改控制台标题
            Console.Title = "Bot";
            Log.Info("Bot初始化", "Bot初始化...");
            //初始化配置文件
            Log.Info("Bot初始化", "初始化服务器全局配置...");
            Log.Warning("Warn", "由于QQ协议的更新以及源包的更新，可能会做出毁灭性的命名调整请查看：https://sora-docs.yukari.one/updatelog/#v1-0-0-rc22以及https://github.com/Mrs4s/go-cqhttp");
            Log.Warning("Warn", "可前往release下载最新版：https://github.com/MuJint/Qiushui-Bot/releases");
            Config config = new(0);
            config.GlobalConfigFileInit();
            config.LoadGlobalConfig(out GlobalConfig globalConfig, false);

            Log.SetLogLevel(LogLevel.Info);
            //显示Log等级
            Log.Debug("Log Level", globalConfig.LogLevel);

            //初始化字符编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //指令匹配初始化
            CommandHelper.KeywordResourseInit();

            //初始化接口对应服务字典
            GlobalSettings.GlobalSettingsInit();

            //
            Log.Info("Bot初始化", "初始化指令匹配集...");
            Log.Info("Bot初始化", "启动反向WebSocket服务器...");
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
