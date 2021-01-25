using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sora.Entities.Base;
using Sora.Entities.CQCodes;
using Sora.Entities.CQCodes.CQCodeModel;
using Sora.Entities.Info;
using Sora.Enumeration.ApiEnum;

namespace Sora.Entities
{
    /// <summary>
    /// 群组实例
    /// </summary>
    public sealed class Group : BaseModel
    {
        #region 属性
        /// <summary>
        /// 群号
        /// </summary>
        public long Id { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器连接标识</param>
        /// <param name="gid">群号</param>
        internal Group(Guid connectionGuid, long gid) : base(connectionGuid)
        {
            this.Id = gid;
        }
        #endregion

        #region 群消息类方法
        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="message">
        /// <para>消息</para>
        /// <para>可以为<see cref="string"/>/<see cref="CQCode"/></para>
        /// <para>其他类型的消息会被强制转换为纯文本</para>
        /// </param>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see langword="messageId"/> 消息ID</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, int messageId)> SendGroupMessage(params object[] message)
        {
            return await base.SoraApi.SendGroupMessage(this.Id, message);
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
        public async ValueTask<(APIStatusType apiStatus, int messageId)> SendGroupMessage(List<CQCode> message)
        {
            return await base.SoraApi.SendGroupMessage(this.Id, message);
        }
        #endregion

        #region 群信息类方法
        /// <summary>
        /// 获取群信息
        /// </summary>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see cref="GroupInfo"/> 群信息</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, GroupInfo groupInfo)> GetGroupInfo(bool useCache = true)
        {
            return await base.SoraApi.GetGroupInfo(this.Id, useCache);
        }

        /// <summary>
        /// 获取群成员列表
        /// </summary>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see cref="List{T}"/> 群成员列表</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, List<GroupMemberInfo> groupMemberList)> GetGroupMemberList()
        {
            return await base.SoraApi.GetGroupMemberList(this.Id);
        }

        /// <summary>
        /// 获取群成员信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see cref="GroupMemberInfo"/> 群成员信息</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, GroupMemberInfo memberInfo)> GetGroupMemberInfo(
            long userId, bool useCache = true)
        {
            return await base.SoraApi.GetGroupMemberInfo(this.Id, userId, useCache);
        }

        #region Go扩展
        /// <summary>
        /// 发送合并转发(群)
        /// 但好像不能用的样子
        /// </summary>
        /// <param name="nodeList">
        /// 节点(<see cref="Node"/>)消息段列表
        /// </param>
        public async ValueTask SendGroupForwardMsg(List<Node> nodeList)
        {
            await base.SoraApi.SendGroupForwardMsg(this.Id, nodeList);
        }

        /// <summary>
        /// 获取群文件系统信息
        /// </summary>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see cref="GroupFileSysInfo"/> 文件系统信息</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, GroupFileSysInfo groupFileSysInfo)> GetGroupFileSysInfo()
        {
            return await SoraApi.GetGroupFileSysInfo(Id);
        }

        /// <summary>
        /// 获取群根目录文件列表
        /// </summary>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see langword="groupFiles"/> 文件列表</para>
        /// <para><see langword="groupFolders"/> 文件夹列表</para>
        /// </returns>
        public async
            ValueTask<(APIStatusType apiStatus, List<GroupFileInfo> groupFiles, List<GroupFolderInfo> groupFolders)>
            GetGroupRootFiles()
        {
            return await SoraApi.GetGroupRootFiles(Id);
        }

        /// <summary>
        /// 获取群根目录文件列表
        /// </summary>
        /// <param name="foldId">文件夹ID</param>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see langword="groupFiles"/> 文件列表</para>
        /// <para><see langword="groupFolders"/> 文件夹列表</para>
        /// </returns>
        public async
            ValueTask<(APIStatusType apiStatus, List<GroupFileInfo> groupFiles, List<GroupFolderInfo> groupFolders)>
            GetGroupFilesByFolder(string foldId)
        {
            return await SoraApi.GetGroupFilesByFolder(Id, foldId);
        }

        /// <summary>
        /// 获取群文件资源链接
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="busid">文件类型</param>
        /// <returns>文件链接</returns>
        public async ValueTask<(APIStatusType apiStatus, string fileUrl)> GetGroupFileUrl(
            string fileId, int busid)
        {
            return await SoraApi.GetGroupFileUrl(Id, fileId, busid);
        }
        #endregion

        #endregion

        #region 群管理方法
        /// <summary>
        /// 设置群组成员禁言
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="duration">
        /// <para>禁言时长(s)</para>
        /// <para>至少60s</para>
        /// </param>
        public async ValueTask EnableGroupMemberMute(long userId, long duration)
        {
            await base.SoraApi.EnableGroupMemberMute(this.Id, userId, duration);
        }

        /// <summary>
        /// 解除群组成员禁言
        /// </summary>
        /// <param name="userId">用户id</param>
        public async ValueTask DisableGroupMemberMute(long userId)
        {
            await base.SoraApi.DisableGroupMemberMute(this.Id, userId);
        }

        /// <summary>
        /// 群组全员禁言
        /// </summary>
        public async ValueTask EnableGroupMute()
        {
            await base.SoraApi.EnableGroupMute(this.Id);
        }

        /// <summary>
        /// 解除群组全员禁言
        /// </summary>s
        public async ValueTask DisableGroupMute()
        {
            await base.SoraApi.DisableGroupMute(this.Id);
        }

        /// <summary>
        /// 设置群成员专属头衔
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="specialTitle">
        /// <para>专属头衔</para>
        /// <para>当值为 <see langword="null"/> 或 <see cref="string"/>.<see langword="Empty"/> 时为清空名片</para>
        /// </param>
        public async ValueTask SetGroupMemberSpecialTitle(long userId, string specialTitle)
        {
            await base.SoraApi.SetGroupMemberSpecialTitle(this.Id, userId, specialTitle);
        }

        /// <summary>
        /// 设置群名片
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="card">
        /// <para>新名片</para>
        /// <para>当值为 <see langword="null"/> 或 <see cref="string"/>.<see langword="Empty"/> 时为清空名片</para>
        /// </param>
        public async ValueTask SetGroupCard(long userId, string card)
        {
            await base.SoraApi.SetGroupCard(this.Id, userId, card);
        }

        /// <summary>
        /// 设置群管理员
        /// </summary>
        /// <param name="userId">成员id</param>
        public async ValueTask EnableGroupAdmin(long userId)
        {
            await base.SoraApi.EnableGroupAdmin(this.Id, userId);
        }

        /// <summary>
        /// 取消群管理员
        /// </summary>
        /// <param name="userId">成员id</param>
        public async ValueTask DisableGroupAdmin(long userId)
        {
            await base.SoraApi.DisableGroupAdmin(this.Id, userId);
        }

        /// <summary>
        /// 退出群
        /// </summary>
        public async ValueTask LeaveGroup()
        {
            await base.SoraApi.LeaveGroup(this.Id);
        }

        /// <summary>
        /// 解散群
        /// </summary>
        public async ValueTask DismissGroup()
        {
            await base.SoraApi.DismissGroup(this.Id);
        }

        /// <summary>
        /// 群组踢人
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="rejectRequest">拒绝此人的加群请求</param>
        public async ValueTask KickGroupMember(long userId, bool rejectRequest = false)
        {
            await base.SoraApi.KickGroupMember(this.Id, userId, rejectRequest);
        }

        #region Go扩展
        /// <summary>
        /// 设置群名
        /// </summary>
        /// <param name="newName">新群名</param>
        public async ValueTask SetGroupName(string newName)
        {
            await base.SoraApi.SetGroupName(this.Id, newName);
        }

        /// <summary>
        /// 设置群头像
        /// </summary>
        /// <param name="imageFile">图片名/绝对路径/URL/base64</param>
        /// <param name="useCache">是否使用缓存</param>
        public async ValueTask SetGroupPortrait(string imageFile, bool useCache = true)
        {
            await base.SoraApi.SetGroupPortrait(this.Id, imageFile, useCache);
        }
        #endregion

        #endregion

        #region 转换方法
        /// <summary>
        /// 定义将 <see cref="Group"/> 对象转换为 <see cref="long"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="Group"/> 对象</param>
        public static implicit operator long (Group value)
        {
            return value.Id;
        }
        #endregion
    }
}
