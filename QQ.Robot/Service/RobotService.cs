using QQ.RoBot.Models;
using Robot.Common;
using Robot.Common.Interface;
using Robot.Framework.Interface;
using Robot.Framework.Models;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QQ.RoBot
{
    /// <summary>
    /// RobotService
    /// </summary>
    public class RobotService : IRobotInterface
    {
        readonly ISignUserServices _userServices;
        readonly ISignLogsServices _logsServices;
        readonly ILianKeyWordsServices _keyWordServices;
        readonly ISpeakerServices _speakerServices;
        readonly ILianInterface _lianService;
        readonly IHsoInterface _hsoInterface;
        readonly ILogsInterface _logs;
        readonly UserConfig userConfig = GlobalSettings.AppSetting.UserConfig;

        public RobotService(ISignUserServices userServices,
            ISignLogsServices logsServices,
            ILianKeyWordsServices keyWordServices,
            ISpeakerServices speakerServices,
            ILianInterface lianService,
            IHsoInterface hsoInterface,
            ILogsInterface logs)
        {
            _userServices = userServices;
            _logsServices = logsServices;
            _keyWordServices = keyWordServices;
            _speakerServices = speakerServices;
            _lianService = lianService;
            _hsoInterface = hsoInterface;
            _logs = logs;
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
                _logs.Error(c, c.Message);
                _logs.Error(c, $"操作失败者：{eventArgs.Operator.Id}");
                await eventArgs.SourceGroup.SendGroupMessage($"[{userInfo.userInfo.Nick}]状态异常");
            }
        }

        public async ValueTask GroupMessageParse(object sender, GroupMessageEventArgs groupMessage)
        {
            if (!IsListenGroup(groupMessage.SourceGroup.Id))
                return;

            await IsAI(groupMessage);
            await Reread(groupMessage);
            await TriggerCute(groupMessage);
            await TriggerSpecial(groupMessage);
            await SpeakerStorage(groupMessage);

            #region 正则匹配关键字Switch分发
            //聊天关键词
            //if (CommandHelper.GetKeywordType(groupMessage.Message.RawText, out KeywordCommand keywordCommand))
            //{
            //    _logs.Info("关键词触发", $"触发关键词[{keywordCommand.GetDescription()}]");
            //    switch (keywordCommand)
            //    {
            //        case KeywordCommand.Hso:
            //            if (userConfig.ModuleSwitch.Hso)
            //                await _hsoInterface.Hso(groupMessage);
            //            break;
            //    }
            //}
            #endregion

            #region 反射利用特性分发

            //从内存中拉取符合匹配函数
            var methodInfo = GlobalSettings.KeyWordRegexs
                            .Where(w => w.Value.Any(regex => regex.IsMatch(groupMessage.Message.RawText)))
                            ?.FirstOrDefault();
            if (methodInfo.HasValue is false || methodInfo.Value.Key is null)
                return;
            _logs.Info($"{methodInfo.GetType()}", $"反射已匹配到方法【{methodInfo.Value.Key}】");

            //获取函数的Attribute
            var methodInfoAttribute = methodInfo.Value.Key.GetCustomAttribute(typeof(KeyWordAttribute));
            if (methodInfoAttribute is KeyWordAttribute keyWordAttribute)
            {
                if(keyWordAttribute.FullMatch)
                {
                    var keyWords = keyWordAttribute.KeyWord.Split(" ").ToList();
                    if (keyWords.Contains(groupMessage.Message.RawText) is false)
                        return;
                }
            }

            //根据接口获取对应的注入服务
            var dicService = GlobalSettings.InterfaceToServicesDic.FirstOrDefault(w => w.Key == GlobalSettings.AllMethods.FirstOrDefault(f => f.Key == methodInfo.Value.Key).Value);
            //匹配服务
            var service = GetInvokeService(dicService.Value);
            var methodParameters = new List<object>() { };
            //组装函数的入参
            foreach (var parameter in methodInfo.Value.Key.GetParameters())
            {
                if (parameter.ParameterType.Name == nameof(Object))
                    methodParameters.Add(sender);
                if (parameter.ParameterType.Name == nameof(GroupMessageEventArgs))
                    methodParameters.Add(groupMessage);
            }
            //调用方法
            methodInfo.Value.Key.Invoke(service, methodParameters.ToArray());
            #endregion
        }

        public async ValueTask GroupPokeEventParse(object sender, GroupPokeEventArgs eventArgs)
        {
            if (eventArgs.TargetUser == eventArgs.LoginUid &&
                   !CheckInCD.IsInCD(eventArgs.SourceGroup, eventArgs.SendUser))
            {
                var msg = new MessageBody()
                {
                    SoraSegment.At(eventArgs.SendUser),
                    SoraSegment.Text("\r\n再戳锤爆你的头\r\n"),
                    SoraSegment.Image("https://i.loli.net/2020/10/20/zWPyocxFEVp2tDT.jpg")
                };
                await eventArgs
                    .SourceGroup
                    .SendGroupMessage(msg);
            }
        }

        public async ValueTask GroupRecallParse(object sender, GroupRecallEventArgs groupMessage)
        {
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
            catch (Exception e)
            {
                _logs.Error(e, "发送撤回消息失败");
            }
        }

        public ValueTask Initalization(object sender, ConnectEventArgs connectEvent)
        {
            _logs.Info("Bot初始化", "与onebot客户端连接成功，初始化资源...");
            //初始化配置文件
            _logs.Info("Bot初始化", $"初始化用户[{connectEvent.LoginUid}]配置");

            //在控制台显示启用模块
            _logs.Info("已启用的模块",
                            $"\n{userConfig.ModuleSwitch}");
            return ValueTask.CompletedTask;
        }

        public async ValueTask PrivateMessageParse(object sender, PrivateMessageEventArgs eventArgs)
        {
            //人工智障
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
        private bool IsListenGroup(long groupId) => userConfig?.ConfigModel?.GroupIds?.Contains(groupId.ToString()) ?? false;

        /// <summary>
        /// 处理ai
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="userConfig"></param>
        /// <returns></returns>
        private async ValueTask IsAI(GroupMessageEventArgs eventArgs)
        {
            //[CQ:at,qq=503745803] 你好啊
            try
            {
                var at = $"[CQ:at,qq={eventArgs.LoginUid}]";
                if (eventArgs.Message.RawText.Contains(at) && userConfig.ModuleSwitch.IsAI)
                {
                    //http://api.qingyunke.com/api.php?key=free&appid=0&msg=关键词
                    var urlEncodeMsg = eventArgs.Message.RawText.Replace(at, "");
                    urlEncodeMsg = HttpUtility.UrlEncode(urlEncodeMsg);
                    //拦截请求 十三分钟限制200次
                    var lastTime = GlobalSettings.AIRequest.Item1;
                    var requestLimit = GlobalSettings.AIRequest.Item2;
                    //重置当前时间，重置请求次数
                    if (lastTime.AddMinutes(13) <= DateTime.Now)
                        GlobalSettings.AIRequest = (DateTime.Now, 0);
                    if (lastTime.AddMinutes(13) >= DateTime.Now && requestLimit >= 200)
                        await eventArgs.Reply($"当前时间段已超出请求次数，请与{lastTime.AddMinutes(13)}之后尝试");
                    GlobalSettings.AIRequest = (lastTime, requestLimit + 1);
                    var json = await RequestAi(userConfig.ConfigModel.AiPath, urlEncodeMsg);
                    if (string.IsNullOrEmpty(json) is false)
                    {
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<AiResult>(json);
                        var msg = new MessageBody()
                        {
                            SoraSegment.At(eventArgs.Sender.Id),
                        };
                        //有结果集
                        if (result.Result == 0)
                        {
                            result.Content = result.Content.Replace("{br}", "\r\n");
                            msg.Add(SoraSegment.Text(result.Content));
                        }
                        await eventArgs.Reply(msg);
                    }
                    else
                        await eventArgs.Reply("网络错误");
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
        private async ValueTask Reread(GroupMessageEventArgs eventArgs)
        {
            //两条一致信息复读，随机数等于6复读
            if (!userConfig.ModuleSwitch.Reread)
                return;
            if (new Random().Next(1, 66) is 6)
                await eventArgs.Repeat();
            else
            {
                var dicCache = GlobalSettings.GetDic;
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
        private async ValueTask TriggerCute(GroupMessageEventArgs eventArgs)
        {
            if (new Random().Next(1, 100) is 66)
            {
                var user = _userServices.QueryById(q => q.QNumber == eventArgs.SenderInfo.UserId.ObjToString());
                var rank = new Random().Next(1, 10);
                if (user == null)
                {
                    var msg = new MessageBody()
                    {
                        SoraSegment.At(eventArgs.SenderInfo.UserId),
                        SoraSegment.Text($"未找到{userConfig.ConfigModel.NickName}任何记录，奖励下发失败~"),
                    };
                    await eventArgs.Reply(msg);
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
                    var msg = new MessageBody()
                    {
                        SoraSegment.At(eventArgs.SenderInfo.UserId),
                        $"看{userConfig.ConfigModel.NickName}这么可爱就奖励{userConfig.ConfigModel.NickName}{rank}分~",
                    };
                    await eventArgs.Reply(msg);
                }
            }
        }

        /// <summary>
        /// 特殊事件
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="userConfig"></param>
        /// <returns></returns>
        private async ValueTask TriggerSpecial(GroupMessageEventArgs eventArgs)
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

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private object GetInvokeService(object obj) => obj switch
        {
            "LianService" => _lianService,
            "HsoService" => _hsoInterface,
            _ => _lianService,
        };
        #endregion

        #region Identity
        private class AiResult
        {
            /// <summary>
            /// 编码
            /// </summary>
            public int Result { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
        }
        #endregion
    }
}
