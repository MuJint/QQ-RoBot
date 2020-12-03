using Qiushui.Lian.Bot.Business;
using Qiushui.Lian.Bot.Framework.IServices;
using Qiushui.Lian.Bot.Framework.Services;
using Qiushui.Lian.Bot.Helper.ConfigModule;
using Qiushui.Lian.Bot.Models;
using Sora.EventArgs.SoraEvent;
using Sora.Tool;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qiushui.Lian.Bot.ServerInterface
{
    /// <summary>
    /// 私聊事件
    /// </summary>
    internal class PrivateMessageEvent
    {
        static readonly ILianKeyWordsServices lianKeyWordsServices = new LianKeyWordsServices();
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
            var all = await lianKeyWordsServices.Query(t => t.ID > 0);
            var result = await lianKeyWordsServices.Query(t => t.Keys.Contains(eventArgs.Message.RawText)) ?? new List<LianKeyWords>();
            if (result.Count > 0 && result != null)
            {
                var strSb = new StringBuilder();
                strSb.Append($"已找到所有符合项：\r\n");
                foreach (var item in result)
                {
                    strSb.Append($"{item.Words}\r\n");
                }
                await eventArgs.Reply(strSb.ToString());
            }
            else
                await eventArgs.Reply("没有找到任何符合要求记录");
            //执行人工智障
            //await eventArgs.Reply("数据密码");
        }
    }
}
