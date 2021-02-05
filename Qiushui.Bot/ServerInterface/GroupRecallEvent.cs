using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace Qiushui.Bot.ServerInterface
{
    internal class GroupRecallEvent
    {

        public static async ValueTask GroupRecallParse(object sender, GroupRecallEventArgs groupMessage)
        {
            //考虑到隐私问题，不写
            await groupMessage.SourceGroup.SendGroupMessage($"怀孕了就说啊，撤回干嘛，大家都会负责的");
        }
    }
}
