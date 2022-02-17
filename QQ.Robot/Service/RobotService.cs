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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        readonly ISpeakerServices _speakerServices;
        readonly ILianInterface _lianService;
        readonly IHsoInterface _hsoInterface;
        readonly ILogsInterface _logs;
        public RobotService()
        {
            _userServices = GetInstance<ISignUserServices>();
            _logsServices = GetInstance<ISignLogsServices>();
            _keyWordServices = GetInstance<ILianKeyWordsServices>();
            _speakerServices = GetInstance<ISpeakerServices>();
            _lianService = GetInstance<ILianInterface>();
            _hsoInterface = GetInstance<IHsoInterface>();
            _logs = GetInstance<ILogsInterface>();
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
            config = new(groupMessage.LoginUid);
            //读取配置文件
            if (!config.LoadUserConfig(out UserConfig userConfig))
            {
                //await groupMessage.Reply("读取配置文件(User)时发生错误\r\n请检查配置文件然后重启");
                _logs.Error(new Exception(), "无法读取用户配置文件");
                return;
            }
            if (!IsListenGroup(groupMessage.SourceGroup.Id, userConfig))
                return;

            await IsAI(groupMessage, userConfig);
            await Reread(groupMessage, userConfig);
            await TriggerCute(groupMessage, userConfig);
            await TriggerSpecial(groupMessage, userConfig);
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
            //        case KeywordCommand.Sign:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.Sign.GetDescription()))
            //                await _lianService.SignIn(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.Search:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.Search.GetDescription()))
            //                await _lianService.SearchRank(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.Fenlai:
            //            await _lianService.Fenlai(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.Skill:
            //            await _lianService.Skill(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.RankList:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.RankList.GetDescription()))
            //                await _lianService.RankList(groupMessage);
            //            break;
            //        case KeywordCommand.SpecialEvent:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.SpecialEvent.GetDescription()))
            //                await _lianService.SpecialEvent(groupMessage);
            //            break;
            //        case KeywordCommand.LogsRecord:
            //            await _lianService.LogsRecord(groupMessage);
            //            break;
            //        case KeywordCommand.Giving:
            //            await _lianService.Giving(groupMessage);
            //            break;
            //        case KeywordCommand.Morning:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.Morning.GetDescription()))
            //                await _lianService.Morning(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.Night:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.Night.GetDescription()))
            //                await _lianService.Night(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.BonusPoint:
            //            await _lianService.BonusPoint(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.DeductPoint:
            //            await _lianService.DeductPoint(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.AllBonusPoint:
            //            await _lianService.AllBonusPoint(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.AllDeductPoint:
            //            await _lianService.AllDeductPoint(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.RedTea:
            //            await _lianService.RedTea(groupMessage);
            //            break;
            //        case KeywordCommand.Raffle:
            //            await _lianService.Raffle(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.Rob:
            //            await _lianService.Rob(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.Rescur:
            //            await _lianService.Rescur(groupMessage, userConfig);
            //            break;
            //        case KeywordCommand.Lian:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.Lian.GetDescription()))
            //                await _lianService.Lian(groupMessage);
            //            break;
            //        case KeywordCommand.AddKeys:
            //            await _lianService.AddKeys(groupMessage);
            //            break;
            //        case KeywordCommand.AddThesaurus:
            //            await _lianService.AddThesaurus(groupMessage);
            //            break;
            //        case KeywordCommand.RollDice:
            //            await _lianService.RollDice(groupMessage);
            //            break;
            //        case KeywordCommand.WordCloud:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.WordCloud.GetDescription()))
            //                await _lianService.WordCloud(groupMessage);
            //            break;
            //        case KeywordCommand.NonsenseKing:
            //            if (IsTrigger(groupMessage.Message.RawText, KeywordCommand.NonsenseKing.GetDescription()))
            //                await _lianService.NonsenseKing(groupMessage);
            //            break;
            //    }
            //}
            #endregion

            #region 反射利用特性分发
            //方法写入内存
            if(GlobalSettings.Methods?.Count<=0)
            {
                var assemblyType = Assembly.GetAssembly(typeof(ILianInterface)).ExportedTypes
                    .Where(w => w.FullName.Contains("Interface"))
                    .Select(s => new AssemblyMethod
                    {
                        Name = s.Name,
                        Methods = s.GetMethods(),
                    }).ToList();
                foreach (var assembly in assemblyType.SelectMany(s => s.Methods))
                {
                    if (assembly?.GetCustomAttribute(typeof(KeyWordAttribute)) is not KeyWordAttribute attribute)
                        continue;
                    //所有反射方法 ILianInterface -> SignIn
                    GlobalSettings.Methods.TryAdd(assembly.DeclaringType.Name, assembly);
                    //正则匹配字典 SignIn -> [(签到)+]  
                    GlobalSettings.KeyWordRegexs.TryAdd(assembly, attribute.KeyWord.Split(' ').Select(s => new Regex($"({s})+")).ToList());
                }
            }
            //从内存中拉取符合匹配函数
            var methodInfo = GlobalSettings.KeyWordRegexs
                            .Where(w => w.Value.Any(regex => regex.IsMatch(groupMessage.Message.RawText)))
                            ?.FirstOrDefault().Key;
            if (methodInfo is null)
                return;
            _logs.Info($"{methodInfo.GetType()}", $"反射已匹配到方法【{methodInfo.Name}】");
            //获取函数的Attribute
            var methodInfoAttribute = methodInfo.GetCustomAttribute(typeof(KeyWordAttribute));
            if (methodInfoAttribute is KeyWordAttribute keyWordAttribute && keyWordAttribute.FullMatch)
            {
                if (keyWordAttribute.KeyWord != groupMessage.Message.RawText)
                    return;
            }
            //根据接口获取对应的注入服务
            var dicService = GlobalSettings.MatchDic.FirstOrDefault(w => w.Key == GlobalSettings.Methods.FirstOrDefault(f => f.Value == methodInfo).Key);
            //匹配服务
            var service = GetInvokeService(dicService.Value);
            var methodParameters = new List<object>() { };
            //组装函数的入参
            foreach (var parameter in methodInfo.GetParameters())
            {
                if (parameter.ParameterType.Name == nameof(Object))
                    methodParameters.Add(sender);
                if (parameter.ParameterType.Name == nameof(GroupMessageEventArgs))
                    methodParameters.Add(groupMessage);
                if (parameter.ParameterType.Name == nameof(UserConfig))
                    methodParameters.Add(userConfig);
            }
            //调用方法
            methodInfo.Invoke(service, methodParameters.ToArray());
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
            //配置文件实例
            config = new(groupMessage.LoginUid);
            //读取配置文件
            if (!config.LoadUserConfig(out UserConfig userConfig))
            {
                //await eventArgs.Sender.SendPrivateMessage("读取配置文件(User)时发生错误\r\n请检查配置文件然后重启");
                _logs.Error(new Exception(), "无法读取用户配置文件");
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
            config = new(connectEvent.LoginUid);
            config.UserConfigFileInit();
            config.LoadUserConfig(out UserConfig userConfig, false);


            //在控制台显示启用模块
            _logs.Info("已启用的模块",
                            $"\n{userConfig.ModuleSwitch}");
            //显示代理信息
            if (userConfig.ModuleSwitch.Hso && !string.IsNullOrEmpty(userConfig.HsoConfig.PximyProxy))
            {
                _logs.Info("Hso Proxy", userConfig.HsoConfig.PximyProxy);
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
                _logs.Error(new Exception(), "无法读取用户配置文件");
                return;
            }

            //人工智障
            //var service = new DealInstruction(_logs, userConfig);
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
                    var msg = new MessageBody()
                    {
                        SoraSegment.At(eventArgs.Sender.Id),
                        SoraSegment.Text(json.ObjectToGBK())
                    };
                    await eventArgs.Reply(msg);
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
        private async ValueTask TriggerCute(GroupMessageEventArgs eventArgs, UserConfig userConfig)
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

        /// <summary>
        /// 反射类
        /// </summary>
        private class AssemblyMethod
        {
            public string Name { get; set; }
            public MethodInfo[] Methods { get; set; }
        }
        private object GetInvokeService(object obj) => obj switch
        {
            "LianService" => _lianService,
            "HsoService" => _hsoInterface,
            _ => _lianService,
        };
        private static bool IsTrigger(string rawText, string keyWord) => rawText.Equals(keyWord);
        #endregion
    }
}
