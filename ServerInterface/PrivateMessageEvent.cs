using Qiushui.Lian.Bot.Business;
using Qiushui.Lian.Bot.Helper.ConfigModule;
using Sora.EventArgs.SoraEvent;
using Sora.Tool;
using System.Threading.Tasks;

namespace Qiushui.Lian.Bot.ServerInterface
{
    /// <summary>
    /// 私聊事件
    /// </summary>
    internal class PrivateMessageEvent
    {
        public static async ValueTask PrivateMessageParse(object sender, PrivateMessageEventArgs eventArgs)
        {
            //配置文件实例
            Config config = new Config(eventArgs.LoginUid);
            //读取配置文件
            if (!config.LoadUserConfig(out UserConfig userConfig))
            {
                await eventArgs.Sender.SendPrivateMessage("读取配置文件(User)时发生错误\r\n请检查配置文件然后重启");
                ConsoleLog.Error("Qiushui机器人管理", "无法读取用户配置文件");
                return;
            }

            //人工智障
            var service = new DealInstruction(userConfig);
            var json = await service.RequestAi(eventArgs.Message.RawText.Replace(" ", ""));
            //处理数据密码
            await eventArgs.Sender.SendPrivateMessage("数据密码");
        }
    }
}
