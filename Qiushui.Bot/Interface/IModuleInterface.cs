using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    /// <summary>
    /// ModuleInterface
    /// <para>模块接口</para>
    /// </summary>
    public interface IModuleInterface
    {
        /// <summary>
        /// 色图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ValueTask HsoHandle(object sender, GroupMessageEventArgs e);

        /// <summary>
        /// lian
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ValueTask LianHandle(object sender, GroupMessageEventArgs e, KeywordCommand command);
    }
}
