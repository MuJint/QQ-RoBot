using System.ComponentModel;

namespace Sora.Enumeration.ApiEnum
{
    internal enum APIType
    {
        #region OnebotAPI
        /// <summary>
        /// 发送消息
        /// </summary>
        [Description("send_msg")]
        SendMsg,
        /// <summary>
        /// 获取登录号信息
        /// </summary>
        [Description("get_login_info")]
        GetLoginInfo,
        /// <summary>
        /// 获取版本信息
        /// </summary>
        [Description("get_version_info")]
        GetVersion,
        /// <summary>
        /// 撤回消息
        /// </summary>
        [Description("delete_msg")]
        RecallMsg,
        /// <summary>
        /// 获取好友列表
        /// </summary>
        [Description("get_friend_list")]
        GetFriendList,
        /// <summary>
        /// 获取群列表
        /// </summary>
        [Description("get_group_list")]
        GetGroupList,
        /// <summary>
        /// 获取群成员信息
        /// </summary>
        [Description("get_group_info")]
        GetGroupInfo,
        /// <summary>
        /// 获取群成员信息
        /// </summary>
        [Description("get_group_member_info")]
        GetGroupMemberInfo,
        /// <summary>
        /// 获取陌生人信息
        /// </summary>
        [Description("get_stranger_info")]
        GetStrangerInfo,
        /// <summary>
        /// 获取群成员列表
        /// </summary>
        [Description("get_group_member_list")]
        GetGroupMemberList,
        /// <summary>
        /// 处理加好友请求
        /// </summary>
        [Description("set_friend_add_request")]
        SetFriendAddRequest,
        /// <summary>
        /// 处理加群请求/邀请
        /// </summary>
        [Description("set_group_add_request")]
        SetGroupAddRequest,
        /// <summary>
        /// 设置群名片
        /// </summary>
        [Description("set_group_card")]
        SetGroupCard,
        /// <summary>
        /// 设置群组专属头衔
        /// </summary>
        [Description("set_group_special_title")]
        SetGroupSpecialTitle,
        /// <summary>
        /// 群组T人
        /// </summary>
        [Description("set_group_kick")]
        SetGroupKick,
        /// <summary>
        /// 群组单人禁言
        /// </summary>
        [Description("set_group_ban")]
        SetGroupBan,
        /// <summary>
        /// 群全体禁言
        /// </summary>
        [Description("set_group_whole_ban")]
        SetGroupWholeBan,
        /// <summary>
        /// 设置群管理员
        /// </summary>
        [Description("set_group_admin")]
        SetGroupAdmin,
        /// <summary>
        /// 群退出
        /// </summary>
        [Description("set_group_leave")]
        SetGroupLeave,
        /// <summary>
        /// 是否可以发送图片
        /// </summary>
        [Description("can_send_image")]
        CanSendImage,
        /// <summary>
        /// 是否可以发送语音
        /// </summary>
        [Description("can_send_record")]
        CanSendRecord,
        /// <summary>
        /// 获取插件运行状态
        /// </summary>
        [Description("get_status")]
        GetStatus,
        /// <summary>
        /// 重启客户端
        /// </summary>
        [Description("set_restart")]
        Restart,
        #endregion

        #region GoAPI
        /// <summary>
        /// 获取图片信息
        /// </summary>
        [Description("get_image")]
        GetImage,
        /// <summary>
        /// 获取消息
        /// </summary>
        [Description("get_msg")]
        GetMessage,
        /// <summary>
        /// 设置群名
        /// </summary>
        [Description("set_group_name")]
        SetGroupName,
        /// <summary>
        /// 获取合并转发消息
        /// </summary>
        [Description("get_forward_msg")]
        GetForwardMessage,
        /// <summary>
        /// 发送合并转发(群)
        /// </summary>
        [Description("send_group_forward_msg ")]
        SendGroupForwardMsg,
        /// <summary>
        /// 设置群头像
        /// </summary>
        [Description("set_group_portrait")]
        SetGroupPortrait,
        /// <summary>
        /// 获取群系统消息
        /// </summary>
        [Description("get_group_system_msg")]
        GetGroupSystemMsg,
        /// <summary>
        /// 获取中文分词
        /// </summary>
        [Description(".get_word_slices")] 
        GetWordSlices,
        /// <summary>
        /// 获取群文件系统信息
        /// </summary>
        [Description("get_group_file_system_info")]
        GetGroupFileSystemInfo,
        /// <summary>
        /// 获取群根目录文件列表
        /// </summary>
        [Description("get_group_root_files")]
        GetGroupRootFiles,
        /// <summary>
        /// 获取群子目录文件列表
        /// </summary>
        [Description("get_group_files_by_folder")]
        GetGroupFilesByFolder,
        /// <summary>
        /// 获取群文件资源链接
        /// </summary>
        [Description("get_group_file_url")]
        GetGroupFileUrl
        #endregion
    }
}
