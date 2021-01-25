using System;
using System.Threading.Tasks;
using Sora.Server.OnebotEvent.RequestEvent;
using Sora.Entities;
using Sora.Enumeration.EventParamsType;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 入群申请
    /// </summary>
    public sealed class AddGroupRequestEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 请求发送者实例
        /// </summary>
        public User Sender { get; private set; }

        /// <summary>
        /// 请求发送到的群组实例
        /// </summary>
        public Group SourceGroup { get; private set; }

        /// <summary>
        /// 验证信息
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// 当前请求的 flag 标识
        /// </summary>
        public string RequsetFlag { get; private set; }

        /// <summary>
        /// 请求子类型
        /// </summary>
        public GroupRequestType SubType { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="groupRequestArgs">加群申请事件参数</param>
        internal AddGroupRequestEventArgs(Guid connectionGuid, string eventName, ApiGroupRequestEventArgs groupRequestArgs) :
            base(connectionGuid, eventName, groupRequestArgs.SelfID, groupRequestArgs.Time)
        {
            this.Sender      = new User(connectionGuid, groupRequestArgs.UserId);
            this.SourceGroup = new Group(connectionGuid, groupRequestArgs.GroupId);
            this.Comment     = groupRequestArgs.Comment;
            this.RequsetFlag = groupRequestArgs.Flag;
            this.SubType     = groupRequestArgs.GroupRequestType;
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 同意当前申请
        /// </summary>
        public async ValueTask Accept()
        {
            await base.SoraApi.SetGroupAddRequest(this.RequsetFlag, this.SubType, true);
        }

        /// <summary>
        /// 拒绝当前申请
        /// </summary>
        /// <param name="reason">原因</param>
        public async ValueTask Reject(string reason = null)
        {
            await base.SoraApi.SetGroupAddRequest(this.RequsetFlag, this.SubType, false, reason);
        }
        #endregion
    }
}
