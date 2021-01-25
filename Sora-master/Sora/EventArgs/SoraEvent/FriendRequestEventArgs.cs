using System;
using System.Threading.Tasks;
using Sora.Server.OnebotEvent.RequestEvent;
using Sora.Entities;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 好友请求事件参数
    /// </summary>
    public sealed class FriendRequestEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 请求发送者实例
        /// </summary>
        public User Sender { get; private set; }

        /// <summary>
        /// 验证信息
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// 当前请求的flag标识
        /// </summary>
        public string RequsetFlag { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="friendRequestArgs">好友申请事件参数</param>
        internal FriendRequestEventArgs(Guid connectionGuid, string eventName,
                                        ApiFriendRequestEventArgs friendRequestArgs) :
            base(connectionGuid, eventName, friendRequestArgs.SelfID, friendRequestArgs.Time)
        {
            this.Sender      = new User(connectionGuid, friendRequestArgs.UserId);
            this.Comment     = friendRequestArgs.Comment;
            this.RequsetFlag = friendRequestArgs.Flag;
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 同意当前申请
        /// </summary>
        /// <param name="remark">设置备注</param>
        public async ValueTask Accept(string remark = null)
        {
            await base.SoraApi.SetFriendAddRequest(this.RequsetFlag, true, remark);
        }

        /// <summary>
        /// 拒绝当前申请
        /// </summary>
        public async ValueTask Reject()
        {
            await base.SoraApi.SetFriendAddRequest(this.RequsetFlag, false);
        }
        #endregion
    }
}
