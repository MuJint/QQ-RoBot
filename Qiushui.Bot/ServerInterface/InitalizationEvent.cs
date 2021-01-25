using Qiushui.Bot.Helper.ConfigModule;
using Sora.EventArgs.SoraEvent;
using Sora.Tool;
using System.Threading.Tasks;

namespace Qiushui.Bot.ServerInterface
{
    /// <summary>
    /// 初始化事件
    /// </summary>
    internal static class InitalizationEvent
    {
        /// <summary>
        /// 初始化处理
        /// </summary>
        internal static ValueTask Initalization(object sender, ConnectEventArgs connectEvent)
        {
            ConsoleLog.Info("Qiushui.Bot初始化","与onebot客户端连接成功，初始化资源...");
            //初始化配置文件
            ConsoleLog.Info("Qiushui.Bot初始化", $"初始化用户[{connectEvent.LoginUid}]配置");
            Config config = new Config(connectEvent.LoginUid);
            config.UserConfigFileInit();
            config.LoadUserConfig(out UserConfig userConfig, false);


            //在控制台显示启用模块
            ConsoleLog.Info("已启用的模块",
                            $"\n{userConfig.ModuleSwitch}");
            //显示代理信息
            if (userConfig.ModuleSwitch.Hso && !string.IsNullOrEmpty(userConfig.HsoConfig.PximyProxy))
            {
                ConsoleLog.Debug("Hso Proxy", userConfig.HsoConfig.PximyProxy);
            }

            //初始化数据库
            //DatabaseInit.Init(connectEvent);

            //初始化定时器线程
            //if (userConfig.ModuleSwitch.Bili_Subscription)
            //{
            //    ConsoleLog.Debug("Timer Init", $"flash span = {userConfig.SubscriptionConfig.FlashTime}");
            //    TimerEventParse.TimerAdd(connectEvent, userConfig.SubscriptionConfig.FlashTime);
            //}

            return ValueTask.CompletedTask;
        }
    }
}
