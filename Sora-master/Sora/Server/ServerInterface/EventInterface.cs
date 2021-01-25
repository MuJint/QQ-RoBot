using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sora.Server.OnebotEvent.MessageEvent;
using Sora.Server.OnebotEvent.MetaEvent;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Server.OnebotEvent.RequestEvent;
using Sora.EventArgs.SoraEvent;
using Sora.Tool;
using Newtonsoft.Json;

namespace Sora.Server.ServerInterface
{
    /// <summary>
    /// Onebot事件接口
    /// 判断和分发基类事件
    /// </summary>
    public class EventInterface
    {
        #region 事件委托

        /// <summary>
        /// Onebot事件回调
        /// </summary>
        /// <typeparam name="TEventArgs">事件参数</typeparam>
        /// <param name="sender">产生事件的客户端</param>
        /// <param name="eventArgs">事件参数</param>
        /// <returns></returns>
        public delegate ValueTask EventAsyncCallBackHandler<in TEventArgs>(object sender, TEventArgs eventArgs)
            where TEventArgs : System.EventArgs;
        #endregion

        #region 事件回调
        /// <summary>
        /// 客户端链接完成事件
        /// </summary>
        public event EventAsyncCallBackHandler<ConnectEventArgs> OnClientConnect;
        /// <summary>
        /// 群聊事件
        /// </summary>
        public event EventAsyncCallBackHandler<GroupMessageEventArgs> OnGroupMessage;
        /// <summary>
        /// 私聊事件
        /// </summary>
        public event EventAsyncCallBackHandler<PrivateMessageEventArgs> OnPrivateMessage;
        /// <summary>
        /// 群申请事件
        /// </summary>
        public event EventAsyncCallBackHandler<AddGroupRequestEventArgs> OnGroupRequest;
        /// <summary>
        /// 好友申请事件
        /// </summary>
        public event EventAsyncCallBackHandler<FriendRequestEventArgs> OnFriendRequest;
        /// <summary>
        /// 群文件上传事件
        /// </summary>
        public event EventAsyncCallBackHandler<FileUploadEventArgs> OnFileUpload;
        /// <summary>
        /// 管理员变动事件
        /// </summary>
        public event EventAsyncCallBackHandler<GroupAdminChangeEventArgs> OnGroupAdminChange;
        /// <summary>
        /// 群成员变动事件
        /// </summary>
        public event EventAsyncCallBackHandler<GroupMemberChangeEventArgs> OnGroupMemberChange;
        /// <summary>
        /// 群成员禁言事件
        /// </summary>
        public event EventAsyncCallBackHandler<GroupMuteEventArgs> OnGroupMemberMute;
        /// <summary>
        /// 好友添加事件
        /// </summary>
        public event EventAsyncCallBackHandler<FriendAddEventArgs> OnFriendAdd;
        /// <summary>
        /// 群聊撤回事件
        /// </summary>
        public event EventAsyncCallBackHandler<GroupRecallEventArgs> OnGroupRecall;
        /// <summary>
        /// 好友撤回事件
        /// </summary>
        public event EventAsyncCallBackHandler<FriendRecallEventArgs> OnFriendRecall;
        /// <summary>
        /// 群名片变更事件
        /// </summary>
        public event EventAsyncCallBackHandler<GroupCardUpdateEventArgs> OnGroupCardUpdate;
        /// <summary>
        /// 群内戳一戳事件
        /// </summary>
        public event EventAsyncCallBackHandler<GroupPokeEventArgs> OnGroupPoke;
        /// <summary>
        /// 运气王事件
        /// </summary>
        public event EventAsyncCallBackHandler<LuckyKingEventArgs> OnLuckyKingEvent;
        /// <summary>
        /// 群成员荣誉变更事件
        /// </summary>
        public event EventAsyncCallBackHandler<HonorEventArgs> OnHonorEvent;
        /// <summary>
        /// 离线文件事件
        /// </summary>
        public event EventAsyncCallBackHandler<OfflineFileEventArgs> OnOfflineFileEvent;  
        #endregion

        #region 事件分发
        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="messageJson">消息json对象</param>
        /// <param name="connection">客户端链接接口</param>
        internal void Adapter(JObject messageJson, Guid connection)
        {
            switch (GetBaseEventType(messageJson))
            {
                //元事件类型
                case "meta_event":
                    MetaAdapter(messageJson, connection);
                    break;
                case "message":
                    MessageAdapter(messageJson, connection);
                    break;
                case "request":
                    RequestAdapter(messageJson, connection);
                    break;
                case "notice":
                    NoticeAdapter(messageJson, connection);
                    break;
                default:
                    //尝试从响应中获取标识符
                    if (messageJson.TryGetValue("echo", out JToken echoJson) &&
                        Guid.TryParse(echoJson.ToString(), out Guid echo)    &&
                        //查找请求标识符是否存在
                        ApiInterface.RequestList.Any(e => e.Echo.Equals(echo)))
                    {
                        //取出返回值中的数据
                        ApiInterface.GetResponse(echo, messageJson);
                        break;
                    }
                    ConsoleLog.Debug("Sora",$"Unknown message :\r{messageJson}");
                    break;
            }
        }
        #endregion

        #region 元事件处理和分发
        /// <summary>
        /// 元事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="connection">连接GUID</param>
        private async void MetaAdapter(JObject messageJson, Guid connection)
        {
            switch (GetMetaEventType(messageJson))
            {
                //心跳包
                case "heartbeat":
                    ApiHeartBeatEventArgs heartBeat = messageJson.ToObject<ApiHeartBeatEventArgs>();
                    ConsoleLog.Debug("Sora",$"Get hreatbeat from [{connection}]");
                    //刷新心跳包记录
                    if (heartBeat != null)
                        ConnectionManager.HeartBeatUpdate(connection);
                    break;
                //生命周期
                case "lifecycle":
                    ApiLifeCycleEventArgs lifeCycle = messageJson.ToObject<ApiLifeCycleEventArgs>();
                    if (lifeCycle != null) ConsoleLog.Debug("Sore", $"Lifecycle event[{lifeCycle.SubType}] from [{connection}]");

                    (int retCode, string clientType, string clientVer) = await ApiInterface.GetClientInfo(connection);
                    if (retCode != 0)//检查返回值
                    {
                        ConsoleLog.Error("Sora",$"获取客户端版本失败(retcode={retCode})");
                        break;
                    }
                    ConsoleLog.Info("Sora",$"已连接到{clientType}客户端,版本:{clientVer}");
                    if(OnClientConnect == null) break;
                    //执行回调函数
                    await OnClientConnect(typeof(EventInterface),
                                          new ConnectEventArgs(connection, "lifecycle",
                                                               lifeCycle?.SelfID ?? -1, clientType, clientVer,
                                                               lifeCycle?.Time   ?? 0));
                    break;
                default:
                    ConsoleLog.Warning("Sora",$"接收到未知事件[{GetMetaEventType(messageJson)}]");
                    break;
            }
        }
        #endregion

        #region 消息事件处理和分发
        /// <summary>
        /// 消息事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="connection">连接GUID</param>
        private async void MessageAdapter(JObject messageJson, Guid connection)
        {
            switch (GetMessageType(messageJson))
            {
                //私聊事件
                case "private":
                    ApiPrivateMsgEventArgs privateMsg = messageJson.ToObject<ApiPrivateMsgEventArgs>();
                    if(privateMsg == null) break;
                    ConsoleLog.Debug("Sora",$"Private msg {privateMsg.SenderInfo.Nick}({privateMsg.UserId}) : {privateMsg.RawMessage}");
                    //执行回调函数
                    if(OnPrivateMessage == null) break;
                    await OnPrivateMessage(typeof(EventInterface),
                                           new PrivateMessageEventArgs(connection, "private", privateMsg));
                    break;
                //群聊事件
                case "group":
                    ApiGroupMsgEventArgs groupMsg = messageJson.ToObject<ApiGroupMsgEventArgs>();
                    if (groupMsg == null) break;
                    ConsoleLog.Debug("Sora",
                                     $"Group msg[{groupMsg.GroupId}] form {groupMsg.SenderInfo.Nick}[{groupMsg.UserId}] : {groupMsg.RawMessage}");
                    //执行回调函数
                    if(OnGroupMessage == null) break;
                    await OnGroupMessage(typeof(EventInterface),
                                         new GroupMessageEventArgs(connection, "group", groupMsg));
                    break;
                default:
                    ConsoleLog.Warning("Sora",$"接收到未知事件[{GetMessageType(messageJson)}]");
                    break;
            }
        }
        #endregion

        #region 请求事件处理和分发
        /// <summary>
        /// 请求事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="connection">连接GUID</param>
        private async void RequestAdapter(JObject messageJson, Guid connection)
        {
            switch (GetRequestType(messageJson))
            {
                //好友请求事件
                case "friend":
                    ApiFriendRequestEventArgs friendRequest = messageJson.ToObject<ApiFriendRequestEventArgs>();
                    if(friendRequest == null)  break;
                    ConsoleLog.Debug("Sora",$"Friend request form [{friendRequest.UserId}] with commont[{friendRequest.Comment}] | flag[{friendRequest.Flag}]");
                    //执行回调函数
                    if(OnFriendRequest == null) break;
                    await OnFriendRequest(typeof(EventInterface),
                                          new FriendRequestEventArgs(connection, "request|friend",
                                                                     friendRequest));
                    break;
                //群组请求事件
                case "group":
                    if (messageJson.TryGetValue("sub_type",out JToken sub) && sub.ToString().Equals("notice"))
                    {
                        ConsoleLog.Warning("Sora","收到notice消息类型，不解析此类型消息");
                        break;
                    }
                    ApiGroupRequestEventArgs groupRequest = messageJson.ToObject<ApiGroupRequestEventArgs>();
                    if(groupRequest == null) break;
                    ConsoleLog.Debug("Sora",$"Group request [{groupRequest.GroupRequestType}] form [{groupRequest.UserId}] with commont[{groupRequest.Comment}] | flag[{groupRequest.Flag}]");
                    //执行回调函数
                    if(OnGroupRequest == null) break;
                    await OnGroupRequest(typeof(EventInterface),
                                         new AddGroupRequestEventArgs(connection, "request|group",
                                                                   groupRequest));
                    break;
                default:
                    ConsoleLog.Warning("Sora",$"接收到未知事件[{GetRequestType(messageJson)}]");
                    break;
            }
        }
        #endregion

        #region 通知事件处理和分发
        /// <summary>
        /// 通知事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="connection">连接GUID</param>
        private async void NoticeAdapter(JObject messageJson, Guid connection)
        {
            switch (GetNoticeType(messageJson))
            {
                //群文件上传
                case "group_upload":
                    ApiFileUploadEventArgs fileUpload = messageJson.ToObject<ApiFileUploadEventArgs>();
                    if(fileUpload == null) break;
                    ConsoleLog.Debug("Sora",
                                     $"Group notice[Upload file] file[{fileUpload.Upload.Name}] from group[{fileUpload.GroupId}({fileUpload.UserId})]");
                    //执行回调函数
                    if(OnFileUpload == null) break;
                    await OnFileUpload(typeof(EventInterface),
                                       new FileUploadEventArgs(connection, "group_upload", fileUpload));
                    break;
                //群管理员变动
                case "group_admin":
                    ApiAdminChangeEventArgs adminChange = messageJson.ToObject<ApiAdminChangeEventArgs>();
                    if(adminChange == null) break;
                    ConsoleLog.Debug("Sora",
                                     $"Group amdin change[{adminChange.SubType}] from group[{adminChange.GroupId}] by[{adminChange.UserId}]");
                    //执行回调函数
                    if(OnGroupAdminChange == null) break;
                    await OnGroupAdminChange(typeof(EventInterface),
                                        new GroupAdminChangeEventArgs(connection, "group_upload", adminChange));
                    break;
                //群成员变动
                case "group_decrease":case "group_increase":
                    ApiGroupMemberChangeEventArgs groupMemberChange = messageJson.ToObject<ApiGroupMemberChangeEventArgs>();
                    if (groupMemberChange == null) break;
                    ConsoleLog.Debug("Sora",
                                     $"{groupMemberChange.NoticeType} type[{groupMemberChange.SubType}] member {groupMemberChange.GroupId}[{groupMemberChange.UserId}]");
                    //执行回调函数
                    if(OnGroupMemberChange == null) break;
                    await OnGroupMemberChange(typeof(EventInterface),
                                              new GroupMemberChangeEventArgs(connection, "group_member_change", groupMemberChange));
                    break;
                //群禁言
                case "group_ban":
                    ApiGroupMuteEventArgs groupMute = messageJson.ToObject<ApiGroupMuteEventArgs>();
                    if (groupMute == null) break;
                    ConsoleLog.Debug("Sora",
                                     $"Group[{groupMute.GroupId}] {groupMute.ActionType} member[{groupMute.UserId}]{groupMute.Duration}");
                    //执行回调函数
                    if(OnGroupMemberMute == null) break;
                    await OnGroupMemberMute(typeof(EventInterface),
                                            new GroupMuteEventArgs(connection, "group_ban", groupMute));
                    break;
                //好友添加
                case "friend_add":
                    ApiFriendAddEventArgs friendAdd = messageJson.ToObject<ApiFriendAddEventArgs>();
                    if(friendAdd == null) break;
                    ConsoleLog.Debug("Sora",$"Friend add user[{friendAdd.UserId}]");
                    //执行回调函数
                    if(OnFriendAdd == null) break;
                    await OnFriendAdd(typeof(EventInterface),
                                      new FriendAddEventArgs(connection, "friend_add", friendAdd));
                    break;
                //群消息撤回
                case "group_recall":
                    ApiGroupRecallEventArgs groupRecall = messageJson.ToObject<ApiGroupRecallEventArgs>();
                    if(groupRecall == null) break;
                    ConsoleLog.Debug("Sora",
                                     $"Group[{groupRecall.GroupId}] recall by [{groupRecall.OperatorId}],msg id={groupRecall.MessageId} sender={groupRecall.UserId}");
                    //执行回调函数
                    if(OnGroupRecall == null) break;
                    await OnGroupRecall(typeof(EventInterface),
                                        new GroupRecallEventArgs(connection, "group_recall", groupRecall));
                    break;
                //好友消息撤回
                case "friend_recall":
                    ApiFriendRecallEventArgs friendRecall = messageJson.ToObject<ApiFriendRecallEventArgs>();
                    if(friendRecall == null) break;
                    ConsoleLog.Debug("Sora", $"Friend[{friendRecall.UserId}] recall msg id={friendRecall.MessageId}");
                    //执行回调函数
                    if(OnFriendRecall == null) break;
                    await OnFriendRecall(typeof(EventInterface),
                                         new FriendRecallEventArgs(connection, "friend_recall", friendRecall));
                    break;
                //群名片变更
                //此事件仅在Go上存在
                case "group_card":
                    ApiGroupCardUpdateEventArgs groupCardUpdate = messageJson.ToObject<ApiGroupCardUpdateEventArgs>();
                    if(groupCardUpdate == null) break;
                    ConsoleLog.Debug("Sora",
                                     $"Group[{groupCardUpdate.GroupId}] member[{groupCardUpdate.UserId}] card update [{groupCardUpdate.OldCard} => {groupCardUpdate.NewCard}]");
                    if (OnGroupCardUpdate == null) break;
                    await OnGroupCardUpdate(typeof(EventInterface),
                                            new GroupCardUpdateEventArgs(connection, "group_card", groupCardUpdate));
                    break;
                case "offline_file":
                    ApiOfflineFileEventArgs offlineFile = messageJson.ToObject<ApiOfflineFileEventArgs>();
                    if (offlineFile == null) break;
                    ConsoleLog.Debug("Sora",
                                     $"Get offline file from[{offlineFile.UserId}] file name = {offlineFile.Info.Name}");
                    if (OnOfflineFileEvent == null) break;
                    await OnOfflineFileEvent(typeof(EventInterface),
                                             new OfflineFileEventArgs(connection, "offline_file", offlineFile));
                    break;
                //通知类事件
                case "notify":
                    switch (GetNotifyType(messageJson))
                    {
                        case "poke"://戳一戳
                            ApiPokeOrLuckyEventArgs pokeEvent = messageJson.ToObject<ApiPokeOrLuckyEventArgs>();
                            if(pokeEvent == null) break;
                            ConsoleLog.Debug("Sora",
                                             $"Group[{pokeEvent.GroupId}] poke from [{pokeEvent.UserId}] to [{pokeEvent.TargetId}]");
                            if(OnGroupPoke == null) break;
                            await OnGroupPoke(typeof(EventInterface),
                                              new GroupPokeEventArgs(connection, "poke", pokeEvent));
                            break;
                        case "lucky_king"://运气王
                            ApiPokeOrLuckyEventArgs luckyEvent = messageJson.ToObject<ApiPokeOrLuckyEventArgs>();
                            if(luckyEvent == null) break;
                            ConsoleLog.Debug("Sora",
                                             $"Group[{luckyEvent.GroupId}] lucky king user[{luckyEvent.TargetId}]");
                            if(OnLuckyKingEvent == null) break;
                            await OnLuckyKingEvent(typeof(EventInterface),
                                                   new LuckyKingEventArgs(connection, "lucky_king", luckyEvent));
                            break;
                        case "honor":
                            ApiHonorEventArgs honorEvent = messageJson.ToObject<ApiHonorEventArgs>();
                            if (honorEvent == null) break;
                            ConsoleLog.Debug("Sora",
                                             $"Group[{honorEvent.GroupId}] member honor change [{honorEvent.HonorType}]");
                            if(OnHonorEvent == null) break;
                            await OnHonorEvent(typeof(EventInterface),
                                               new HonorEventArgs(connection, "honor", honorEvent));
                            break;
                        default:
                            ConsoleLog.Warning("Sora",$"未知Notify事件类型[{GetNotifyType(messageJson)}]");
                            break;
                    }
                    break;
                default:
                    ConsoleLog.Debug("Sora",$"unknown notice \n{messageJson}");
                    ConsoleLog.Warning("Sora",$"接收到未知事件[{GetNoticeType(messageJson)}]");
                    break;
            }
        }
        #endregion

        #region 事件类型获取
        /// <summary>
        /// 获取上报事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetBaseEventType(JObject messageJson) =>
            !messageJson.TryGetValue("post_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取元事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetMetaEventType(JObject messageJson) =>
            !messageJson.TryGetValue("meta_event_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取消息事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetMessageType(JObject messageJson) =>
            !messageJson.TryGetValue("message_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取请求事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetRequestType(JObject messageJson) =>
            !messageJson.TryGetValue("request_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取通知事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetNoticeType(JObject messageJson) =>
            !messageJson.TryGetValue("notice_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取通知事件子类型
        /// </summary>
        /// <param name="messageJson"></param>
        private static string GetNotifyType(JObject messageJson) =>
            !messageJson.TryGetValue("sub_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        #endregion
    }
}
