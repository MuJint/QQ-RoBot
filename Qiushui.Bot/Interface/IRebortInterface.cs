using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace Qiushui.Bot
{
    /// <summary>
    /// RebortEvent interface
    /// </summary>
    public interface IRebortInterface
    {
        /// <summary>
        /// Initalization
        /// <para>初始化</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connectEvent"></param>
        /// <returns></returns>
        ValueTask Initalization(object sender, ConnectEventArgs connectEvent);
        /// <summary>
        /// GroupMessage
        /// <para>群组消息</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="groupMessage"></param>
        /// <returns></returns>
        ValueTask GroupMessageParse(object sender, GroupMessageEventArgs groupMessage);
        /// <summary>
        /// PrivateMessage
        /// <para>私聊消息</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        ValueTask PrivateMessageParse(object sender, PrivateMessageEventArgs eventArgs);
        /// <summary>
        /// GroupPoke
        /// <para>群戳一戳</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        ValueTask GroupPokeEventParse(object sender, GroupPokeEventArgs eventArgs);
        /// <summary>
        /// FriendAdd
        /// <para>新朋友</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        ValueTask FriendAddParse(object sender, FriendAddEventArgs eventArgs);
        /// <summary>
        /// GroupMemberChange
        /// <para>群组成员变动</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        ValueTask GroupMemberChangeParse(object sender, GroupMemberChangeEventArgs eventArgs);
        /// <summary>
        /// GroupRecall
        /// <para>群消息撤回</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="groupMessage"></param>
        /// <returns></returns>
        ValueTask GroupRecallParse(object sender, GroupRecallEventArgs groupMessage);
    }
}
