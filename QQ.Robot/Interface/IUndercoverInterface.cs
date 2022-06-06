using Robot.Common;
using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    /// <summary>
    /// IUndercoverInterface
    /// <para>谁是卧底</para>
    /// </summary>
    public interface IUndercoverInterface
    {
        /// <summary>
        /// 谁是卧底
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [KeyWord("谁是卧底", true)]
        ValueTask Undercover(GroupMessageEventArgs eventArgs);
        /// <summary>
        /// 加入谁是卧底
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [KeyWord("jrwd")]
        ValueTask JoinGame(GroupMessageEventArgs eventArgs);
        /// <summary>
        /// 谁是卧底房间列表
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [KeyWord("sswd", true)]
        ValueTask RoomList(GroupMessageEventArgs eventArgs);
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        ValueTask StartGame(GroupMessageEventArgs eventArgs, int roomId);
        /// <summary>
        /// 结束游戏
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="undercoverWin">卧底赢？</param>
        /// <returns></returns>
        ValueTask StopGame(GroupMessageEventArgs eventArgs, bool undercoverWin = false);
        /// <summary>
        /// 解散房间
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [KeyWord("jsfj")]
        ValueTask DissolveRoom(GroupMessageEventArgs eventArgs);
        /// <summary>
        /// 发起投票
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [KeyWord("fqtp", true)]
        ValueTask Fqtp(GroupMessageEventArgs eventArgs);
        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [KeyWord("tpy")]
        ValueTask Vote(GroupMessageEventArgs eventArgs);
    }
}
