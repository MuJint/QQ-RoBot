using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sora.Entities.Base;
using Sora.Entities.CQCodes;
using Sora.Entities.Info;
using Sora.Enumeration.ApiEnum;

namespace Sora.Entities
{
    /// <summary>
    /// 用户类
    /// </summary>
    public sealed class User : BaseModel
    {
        #region 属性
        /// <summary>
        /// 当前实例的用户ID
        /// </summary>
        public long Id { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器连接标识</param>
        /// <param name="uid">用户ID</param>
        internal User(Guid connectionGuid, long uid) : base(connectionGuid)
        {
            this.Id = uid;
        }
        #endregion

        #region 常用操作方法
        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="message">
        /// <para>消息</para>
        /// <para>可以为<see cref="string"/>/<see cref="CQCode"/>/<see cref="List{T}"/>(T = <see cref="CQCode"/>)</para>
        /// <para>其他类型的消息会被强制转换为纯文本</para>
        /// </param>
        public async ValueTask<(APIStatusType apiStatus,int message)> SendPrivateMessage(params object[] message)
        {
            return await base.SoraApi.SendPrivateMessage(this.Id, message);
        }

        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="message">
        /// <para>消息</para>
        /// <para><see cref="List{T}"/>(T = <see cref="CQCode"/>)</para>
        /// <para>其他类型的消息会被强制转换为纯文本</para>
        /// </param>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see langword="messageId"/> 消息ID</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, int messageId)> SendPrivateMessage(List<CQCode> message)
        {
            return await base.SoraApi.SendPrivateMessage(this.Id, message);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="useCache"></param>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see cref="UserInfo"/> 群成员信息</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, UserInfo userInfo)> GetUserInfo(bool useCache = true)
        {
            return await base.SoraApi.GetUserInfo(this.Id, useCache);
        }
        #endregion

        #region CQ码方法
        /// <summary>
        /// 获取At的CQ码
        /// </summary>
        /// <returns>
        /// <see cref="CQCode"/> AT
        /// </returns>
        public CQCode CQCodeAt()
        {
            return CQCode.CQAt(this.Id);
        }
        #endregion

        #region 转换方法
        /// <summary>
        /// 定义将 <see cref="User"/> 对象转换为 <see cref="long"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="User"/> 对象</param>
        public static implicit operator long (User value)
        {
            return value.Id;
        }

        /// <summary>
        /// 定义将 <see cref="User"/> 对象转换为 <see cref="string"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="User"/> 对象</param>
        public static implicit operator string (User value)
        {
            return value.ToString ();
        }
        #endregion
    }
}
