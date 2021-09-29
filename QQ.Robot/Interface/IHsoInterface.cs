using Robot.Common;
using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    /// <summary>
    /// IHsoInterface
    /// </summary>
    public interface IHsoInterface
    {
        /// <summary>
        /// 色图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [KeyWord("色图 来点色图 涩图 来点涩图")]
        ValueTask Hso(GroupMessageEventArgs e);
    }
}
