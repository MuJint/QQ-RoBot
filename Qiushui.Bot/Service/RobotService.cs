using Robot.Common;
using Robot.Framework.Interface;
using Robot.Framework.Models;
using Sora.Entities.MessageElement;
using Sora.EventArgs.SoraEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukariToolBox.FormatLog;

namespace QQ.RoBot
{
    /// <summary>
    /// RobotService
    /// </summary>
    public class RobotService : BaseServiceObject, IRobotInterface
    {
        private Config config;
        readonly ISignUserServices _userServices;
        readonly ISignLogsServices _logsServices;
        readonly ILianKeyWordsServices _keyWordServices;
        readonly IModuleInterface _moduleInterface;
        readonly ISpeakerServices _speakerServices;
        public RobotService()
        {
            _userServices = GetInstance<ISignUserServices>();
            _logsServices = GetInstance<ISignLogsServices>();
            _keyWordServices = GetInstance<ILianKeyWordsServices>();
            _moduleInterface = GetInstance<IModuleInterface>();
            _speakerServices = GetInstance<ISpeakerServices>();
        }
        public ValueTask FriendAddParse(object sender, FriendAddEventArgs eventArgs)
        {
            //throw new NotImplementedException();
            return ValueTask.CompletedTask;
        }

        public async ValueTask GroupMemberChangeParse(object sender, GroupMemberChangeEventArgs eventArgs)
        {
            var userInfo = await eventArgs.ChangedUser.GetUserInfo();
            try
            {
                var @operator = await eventArgs.Operator.GetUserInfo();
                switch (eventArgs.SubType)
                {
                    case Sora.Enumeration.EventParamsType.MemberChangeType.Leave:
                        await eventArgs.SourceGroup.SendGroupMessage($"[{userInfo.userInfo.Nick}]退群了。江湖路远，有缘再会");
                        break;
                    case Sora.Enumeration.EventParamsType.MemberChangeType.Kick:
                        await eventArgs.SourceGroup.SendGroupMessage($"[{userInfo.userInfo.Nick}]被[{@operator.userInfo.Nick}]强行请离了。江湖路远，有缘再会");
                        break;
                    case Sora.Enumeration.EventParamsType.MemberChangeType.KickMe:
                        break;
                    case Sora.Enumeration.EventParamsType.MemberChangeType.Approve:
                        await eventArgs.SourceGroup.SendGroupMessage($"[{@operator.userInfo.Nick}]同意[{userInfo.userInfo.Nick}]加入了群聊");
                        break;
                    case Sora.Enumeration.EventParamsType.MemberChangeType.Invite:
                        await eventArgs.SourceGroup.SendGroupMessage($"[{@operator.userInfo.Nick}]邀请[{userInfo.userInfo.Nick}]加入了群聊");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception c)
            {
                //此处存在一个异常
                //System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. (Parameter 'userId')
                Log.Error("Error", c.Message);
                Log.Error("Error", $"操作失败者：{eventArgs.Operator.Id}");
                await eventArgs.SourceGroup.SendGroupMessage($"[{userInfo.userInfo.Nick}]状态异常");
            }
        }

        public async ValueTask GroupMessageParse(object sender, GroupMessageEventArgs groupMessage)
        {
            config = new(groupMessage.LoginUid);
            //读取配置文件
            if (!config.LoadUserConfig(out UserConfig userConfig))
            {
                //await groupMessage.Reply("读取配置文件(User)时发生错误\r\n请检查配置文件然后重启");
                Log.Error("Qiushui机器人管理", "无法读取用户配置文件");
                return;
            }
            if (!IsListenGroup(groupMessage.SourceGroup.Id, userConfig))
                return;

            await IsAI(groupMessage, userConfig);
            await Reread(groupMessage, userConfig);
            await TriggerCute(groupMessage, userConfig);
            await TriggerSpecial(groupMessage, userConfig);
            await SpeakerStorage(groupMessage);

            //聊天关键词
            if (CommandHelper.GetKeywordType(groupMessage.Message.RawText, out KeywordCommand keywordCommand))
            {
                Log.Info("关键词触发", $"触发关键词[{keywordCommand.GetDescription()}]");
                switch (keywordCommand)
                {
                    case KeywordCommand.Hso:
                        if (userConfig.ModuleSwitch.Hso)
                            await _moduleInterface.HsoHandle(sender, groupMessage);
                        break;
                    default:
                        if (userConfig.ModuleSwitch.LianBot)
                            await _moduleInterface.LianHandle(sender, groupMessage, keywordCommand);
                        break;
                }
            }
        }

        public async ValueTask GroupPokeEventParse(object sender, GroupPokeEventArgs eventArgs)
        {
            if (eventArgs.TargetUser == eventArgs.LoginUid &&
                   !CheckInCD.IsInCD(eventArgs.SourceGroup, eventArgs.SendUser))
            {
                await eventArgs
                    .SourceGroup
                    .SendGroupMessage($"{CQCodes.CQAt(eventArgs.SendUser)}\r\n再戳锤爆你的头\r\n{CQCodes.CQImage("https://i.loli.net/2020/10/20/zWPyocxFEVp2tDT.jpg")}");
            }
        }

        public async ValueTask GroupRecallParse(object sender, GroupRecallEventArgs groupMessage)
        {
            //配置文件实例
            config = new(groupMessage.MessageSender.Id);
            //读取配置文件
            if (!config.LoadUserConfig(out UserConfig userConfig))
            {
                //await eventArgs.Sender.SendPrivateMessage("读取配置文件(User)时发生错误\r\n请检查配置文件然后重启");
                Log.Error("Bot机器人管理", "无法读取用户配置文件");
                return;
            }
            try
            {
                var r = new Random().Next(5, 9);
                if (r is 6)
                {
                    var msg = _speakerServices.Query(s => s.MsgId == groupMessage.MessageId).First();
                    var user = _userServices.Query(s => s.QNumber == groupMessage.MessageSender.Id.ObjToString()).First();
                    await groupMessage.SourceGroup.SendGroupMessage($"[有人撤回了消息，但我要说]\r\n[时间：{msg.CreateTime:HH:mm:ss}]\r\n[昵称：{user.NickName}]\r\n[ID：{user.QNumber}]\r\n以下消息正文\r\n{msg.RawText}");
                }
                else if (userConfig.ModuleSwitch.Recal)
                {
                    var msg = _speakerServices.Query(s => s.MsgId == groupMessage.MessageId).First();
                    var user = _userServices.Query(s => s.QNumber == groupMessage.MessageSender.Id.ObjToString()).First();
                    await groupMessage.SourceGroup.SendGroupMessage($"[有人撤回了消息，但我要说]\r\n[时间：{msg.CreateTime:HH:mm:ss}]\r\n[昵称：{user.NickName}]\r\n[ID：{user.QNumber}]\r\n以下消息正文\r\n{msg.RawText}");
                }
                else
                    await groupMessage.SourceGroup.SendGroupMessage($"怀孕了就说啊，撤回干嘛，大家都会负责的");
            }
            catch (Exception)
            {
                await groupMessage.SourceGroup.SendGroupMessage($"我感觉他撤回的是图片，我懒得弄，下次一定");
            }
        }

        public ValueTask Initalization(object sender, ConnectEventArgs connectEvent)
        {
            Log.Info("Bot初始化", "与onebot客户端连接成功，初始化资源...");
            //初始化配置文件
            Log.Info("Bot初始化", $"初始化用户[{connectEvent.LoginUid}]配置");
            config = new(connectEvent.LoginUid);
            config.UserConfigFileInit();
            config.LoadUserConfig(out UserConfig userConfig, false);


            //在控制台显示启用模块
            Log.Info("已启用的模块",
                            $"\n{userConfig.ModuleSwitch}");
            //显示代理信息
            if (userConfig.ModuleSwitch.Hso && !string.IsNullOrEmpty(userConfig.HsoConfig.PximyProxy))
            {
                Log.Debug("Hso Proxy", userConfig.HsoConfig.PximyProxy);
            }

            return ValueTask.CompletedTask;
        }

        public async ValueTask PrivateMessageParse(object sender, PrivateMessageEventArgs eventArgs)
        {
            //配置文件实例
            config = new(eventArgs.LoginUid);
            //读取配置文件
            if (!config.LoadUserConfig(out UserConfig userConfig))
            {
                //await eventArgs.Sender.SendPrivateMessage("读取配置文件(User)时发生错误\r\n请检查配置文件然后重启");
                Log.Error("Bot机器人管理", "无法读取用户配置文件");
                return;
            }

            //人工智障
            //var service = new DealInstruction(Log, userConfig);
            var all = _keyWordServices.Query(t => t.ID > 0);
            var result = _keyWordServices.Query(t => t.Keys.Contains(eventArgs.Message.RawText)) ?? new List<LianKeyWords>();
            if (result.Count > 0 && result != null)
            {
                var strSb = new StringBuilder();
                strSb.Append($"已找到所有符合项：\r\n");
                foreach (var item in result)
                {
                    strSb.Append($"{item.Words}\r\n");
                }
                await eventArgs.Reply(strSb.ToString());
            }
            else
                await eventArgs.Reply("没有找到任何符合要求记录");
            //执行人工智障
            //await eventArgs.Reply("数据密码");
        }


        #region Func
        /// <summary>
        /// 返回是否在已监听列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private static bool IsListenGroup(long groupId, UserConfig userConfig) => userConfig?.ConfigModel?.GroupIds?.Contains(groupId.ToString()) ?? false;

        /// <summary>
        /// 处理ai
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="userConfig"></param>
        /// <returns></returns>
        private static async ValueTask IsAI(GroupMessageEventArgs eventArgs, UserConfig userConfig)
        {
            //[CQ:at,qq=503745803] 你好啊
            try
            {
                var at = $"[CQ:at,qq={eventArgs.LoginUid}]";
                if (eventArgs.Message.RawText.Contains(at) && userConfig.ModuleSwitch.IsAI)
                {
                    var json = await RequestAi(userConfig.ConfigModel.AiPath,
                        eventArgs.Message.RawText.Replace(at, "").Replace(" ", ""));
                    await eventArgs.Reply($"{CQCodes.CQAt(eventArgs.Sender.Id)}{json.ObjectToGBK()}");
                }
            }
            catch (Exception c)
            {
                await eventArgs.Reply(c.Message);
            }
        }

        /// <summary>
        /// 复读姬
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="userConfig"></param>
        /// <returns></returns>
        private static async ValueTask Reread(GroupMessageEventArgs eventArgs, UserConfig userConfig)
        {
            //两条一致信息复读，随机数等于6复读
            if (!userConfig.ModuleSwitch.Reread)
                return;
            if (new Random().Next(1, 66) is 6)
                await eventArgs.Repeat();
            else
            {
                var dicCache = StaticModel.GetDic;
                if (dicCache.ContainsKey(eventArgs.SourceGroup.Id))
                {
                    dicCache.TryGetValue(eventArgs.SourceGroup.Id, out var dicResult);
                    if (dicResult.Count <= 2 && dicResult.All(t => t.Equals(eventArgs.Message.RawText)))
                        await eventArgs.Repeat();   //复读，两条一致信息
                    if (dicResult.Count <= 2)
                        dicCache.Remove(eventArgs.SourceGroup.Id);
                    else
                    {
                        dicResult.Add(eventArgs.Message.RawText);
                        dicCache[eventArgs.SourceGroup.Id] = dicResult;
                    }
                }
                else
                    dicCache.Add(eventArgs.SourceGroup.Id, new List<string>() { eventArgs.Message.RawText });
            }
        }

        /// <summary>
        /// 触发加分
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="userConfig"></param>
        /// <returns></returns>
        private async ValueTask TriggerCute(GroupMessageEventArgs eventArgs, UserConfig userConfig)
        {
            if (new Random().Next(1, 100) is 66)
            {
                var user = _userServices.QueryById(q => q.QNumber == eventArgs.SenderInfo.UserId.ObjToString());
                var rank = new Random().Next(1, 10);
                if (user == null)
                {
                    await eventArgs.Reply($"{CQCodes.CQAt(eventArgs.SenderInfo.UserId)}未找到{userConfig.ConfigModel.NickName}任何记录，奖励下发失败~");
                }
                else
                {
                    user.Rank += rank;
                    user.LastModifyTime = DateTime.Now;
                    _userServices.Update(user);
                    _logsServices.Insert(new SignLogs()
                    {
                        CmdType = CmdType.BonusPoints,
                        LogContent = $"[可爱]指令赠送{rank}分",
                        ModifyRank = rank,
                        Uid = eventArgs.SenderInfo.UserId.ObjToString()
                    });
                    await eventArgs.Reply($"{CQCodes.CQAt(eventArgs.SenderInfo.UserId)}看{userConfig.ConfigModel.NickName}这么可爱就奖励{userConfig.ConfigModel.NickName}{rank}分~");
                }
            }
        }

        /// <summary>
        /// 特殊事件
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="userConfig"></param>
        /// <returns></returns>
        private async ValueTask TriggerSpecial(GroupMessageEventArgs eventArgs, UserConfig userConfig)
        {
            var deTrigger = new Random().Next(1, 5000) is 666;
            var trigger = new Random().Next(1, 5000) is 444;
            var rank = new Random().Next(7, 12);
            var strSb = new StringBuilder();
            var uList = _userServices.Query(s => s.Status == Status.Valid);
            if (deTrigger)
            {
                uList.ForEach((item) =>
                {
                    item.Rank -= rank;
                    item.LastModifyTime = DateTime.Now;
                    _userServices.Update(item);
                });
                strSb.Append($"[江湖传言]\r\n");
                strSb.Append($"恭喜{userConfig.ConfigModel.NickName}[{eventArgs.SenderInfo.Nick}]通过挖宝拾取道具：陨焰之盒 × 1\r\n");
                strSb.Append($"就你有手？奖励所有{userConfig.ConfigModel.NickName}负{rank}分");
                _logsServices.Insert(new SignLogs()
                {
                    CmdType = CmdType.SpecialPointsDeducted,
                    LogContent = $"[{eventArgs.SenderInfo.Nick}]就你有手？奖励所有{userConfig.ConfigModel.NickName}负{rank}分",
                    Uid = eventArgs.LoginUid.ObjToString(),
                    ModifyRank = rank
                });
                await eventArgs.Reply(strSb.ToString());
            }
            if (trigger)
            {
                uList.ForEach((item) =>
                {
                    item.Rank += rank;
                    item.LastModifyTime = DateTime.Now;
                    _userServices.Update(item);
                });
                strSb.Append($"[江湖传言]\r\n");
                strSb.Append($"恭喜{userConfig.ConfigModel.NickName}[{userConfig.ConfigModel.BotName}]通过挖宝拾取道具：陨焰之盒 × 1\r\n");
                strSb.Append($"普天同庆！！！奖励所有{userConfig.ConfigModel.NickName}{rank}分");
                _logsServices.Insert(new SignLogs()
                {
                    CmdType = CmdType.SpecialBonusPoints,
                    LogContent = $"普天同庆！！！奖励所有{userConfig.ConfigModel.NickName}{rank}分",
                    Uid = eventArgs.LoginUid.ObjToString(),
                    ModifyRank = rank
                });
                await eventArgs.Reply(strSb.ToString());
            }
        }

        /// <summary>
        /// 人工智障请求
        /// 未涉及到机器学习
        /// </summary>
        /// <param name="str"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static async ValueTask<string> RequestAi(string path, string str) => await HttpHelper.HttpGetAsync($"{path}/{str}");

        /// <summary>
        /// 消息入库
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        private async ValueTask SpeakerStorage(GroupMessageEventArgs eventArgs)
        {
            //[CQ:image,file=bc99f5e35895cf9a48edd43a682527a4.image]
            await Task.Run(() =>
            {
                if (eventArgs.Message.RawText.StartsWith("[CQ:image,file="))
                {
                    _speakerServices.Insert(new SpeakerList()
                    {
                        GroupId = eventArgs.SourceGroup.Id,
                        RawText = eventArgs.Message.RawText.Replace("[CQ:image,file=", "").Replace("]", ""),
                        MsgId = eventArgs.Message.MessageId,
                        Uid = eventArgs.Sender.Id,
                        MsgType = MsgType.Img
                    });
                }
                else
                {
                    _speakerServices.Insert(new SpeakerList()
                    {
                        GroupId = eventArgs.SourceGroup.Id,
                        RawText = eventArgs.Message.RawText,
                        MsgId = eventArgs.Message.MessageId,
                        Uid = eventArgs.Sender.Id
                    });
                }
            });
        }
        #endregion
    }
}
