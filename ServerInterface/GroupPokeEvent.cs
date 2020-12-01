using Qiushui.Lian.Bot.Helper;
using Sora.Entities.CQCodes;
using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace Qiushui.Lian.Bot.ServerInterface
{
    /// <summary>
    /// 戳一戳
    /// </summary>
    internal class GroupPokeEvent
    {
        public static async ValueTask GroupPokeEventParse(object sender, GroupPokeEventArgs eventArgs)
        {
            if (eventArgs.TargetUser == eventArgs.LoginUid &&
                !CheckInCD.IsInCD(eventArgs.SourceGroup, eventArgs.SendUser))
            {
                await eventArgs
                    .SourceGroup
                    .SendGroupMessage(
                        CQCode.CQAt(eventArgs.SendUser),
                        "\r\n再戳锤爆你的头\r\n",
                        CQCode.CQImage("https://i.loli.net/2020/10/20/zWPyocxFEVp2tDT.jpg"));
            }
        }
    }
}
