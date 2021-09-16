using JiebaNet.Segmenter;
using JiebaNet.Segmenter.Common;
using Robot.Common;
using Robot.Framework.Interface;
using Robot.Framework.Models;
using Sora.Entities;
using Sora.Entities.MessageElement;
using Sora.EventArgs.SoraEvent;
using System;
using System.Collections.Generic;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    /// <summary>
    /// LianService
    /// <para>天刀整容团莲的实现</para>
    /// </summary>
    public class LianService : BaseServiceObject, ILianInterface
    {
        readonly ISignUserServices _signUserServices;
        readonly ISignLogsServices _signLogsServices;
        readonly ILianChatServices _lianChatServices;
        readonly ILianKeyWordsServices _lianKeyWordsServices;
        readonly ISpeakerServices _speakerServices;
        public LianService()
        {
            _signUserServices = GetInstance<ISignUserServices>();
            _signLogsServices = GetInstance<ISignLogsServices>();
            _lianChatServices = GetInstance<ILianChatServices>();
            _lianKeyWordsServices = GetInstance<ILianKeyWordsServices>();
            _speakerServices = GetInstance<ISpeakerServices>();
        }
        #region 签到方法
        /// <summary>
        /// 签到方法
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask SignIn(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            var isSign = RequestUsers(eventArgs.SenderInfo.UserId);
            int randRank = new Random().Next(2, 5);
            if (isSign != null)
            {
                var signLog = (_signLogsServices.Query(t => t.CmdType == CmdType.SignIn && t.Uid.Equals(eventArgs.SenderInfo.UserId.ObjToString()))).OrderByDescending(t => t.LastModifyTime)?.FirstOrDefault() ?? new SignLogs();
                if (signLog.LastModifyTime.DayOfYear == DateTime.Now.DayOfYear && signLog.ModifyRank > 0)
                {
                    if (TriggerPunish)
                    {
                        await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.Sender.Id, 3 * 60);
                        await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}不要重复签到yo~惊不惊喜，意不意外？", true);
                    }
                    else
                        await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}不要重复签到鸭，不然{config.ConfigModel.BotName}会打飞你的头嗷~", true);
                }
                else
                {
                    isSign.LastModifyTime = DateTime.Now;
                    isSign.Rank += randRank;
                    isSign.GroupId = eventArgs.SourceGroup.Id.ObjToString();
                    isSign.NickName = eventArgs.SenderInfo.Nick ?? "";
                    RequestSignAsync(isSign, true);
                    RequestLogsAsync(new SignLogs()
                    {
                        LogContent = $"[签到]成功赠送积分{randRank}",
                        Uid = eventArgs.SenderInfo.UserId.ObjToString(),
                        ModifyRank = randRank
                    });
                    await SendMessageGroup(eventArgs, $"恭喜{config.ConfigModel.NickName}签到成功，奖励{randRank}分{config.ConfigModel.Tail}", true);
                }
            }
            else
            {
                //初次无记录
                RequestSignAsync(new SignUser()
                {
                    GroupId = eventArgs.SourceGroup.Id.ObjToString(),
                    NickName = eventArgs.SenderInfo.Nick,
                    QNumber = eventArgs.SenderInfo.UserId.ObjToString(),
                    Rank = randRank
                });
                RequestLogsAsync(new SignLogs()
                {
                    LogContent = $"[签到]成功赠送积分{randRank}",
                    Uid = eventArgs.SenderInfo.UserId.ObjToString(),
                    ModifyRank = randRank
                });
                await SendMessageGroup(eventArgs, $"恭喜{config.ConfigModel.NickName}签到成功，奖励{randRank}分{config.ConfigModel.Tail}", true);
            }
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask SearchRank(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            var isSign = _signUserServices.QueryById(t => t.QNumber.Equals(eventArgs.SenderInfo.UserId.ObjToString()));
            if (isSign != null)
                await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}当前有{isSign.Rank}分，继续努力吧~", true);
            else
                await SendMessageGroup(eventArgs, $"没有找到任何记录噢~请先对{config.ConfigModel.BotName}说签到吧", true);
        }
        #endregion

        #region 分来
        /// <summary>
        /// 分来
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Fenlai(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            int randRank = new Random().Next(2, 5);
            if (new Random().Next(1, 100) is 6)
            {
                var isSign = _signUserServices.QueryById(t => t.QNumber.Equals(eventArgs.SenderInfo.UserId.ObjToString()));
                if (isSign != null)
                    await SendMessageGroup(eventArgs, $"没有找到任何记录噢~请先对{config.ConfigModel.BotName}说签到吧", true);
                else
                {
                    isSign.Rank += randRank;
                    isSign.LastModifyTime = DateTime.Now;
                    RequestSignAsync(isSign, true);
                    RequestLogsAsync(new SignLogs()
                    {
                        CmdType = CmdType.BonusPoints,
                        LogContent = $"[分来]指令赠送{randRank}分",
                        ModifyRank = randRank,
                        Uid = eventArgs.Sender.Id.ObjToString()
                    });
                    await SendMessageGroup(eventArgs, $"看{config.ConfigModel.NickName}这么可爱，就送{config.ConfigModel.NickName}{randRank}分吧~", true);
                }
            }
            else
                await SendMessageGroup(eventArgs, $"走开啦，丑逼~", true);
        }
        #endregion

        #region 说明
        /// <summary>
        /// 说明
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Skill(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            var strSb = new StringBuilder();
            strSb.Append($"[当前机器人]：{config.ConfigModel.BotName}\r\n");
            strSb.Append($"[开源地址]：https://github.com/MuJint/Qiushui-Bot.git \r\n");
            strSb.Append($"[常用指令]：https://github.com/MuJint/Qiushui-Bot/tree/master#%E5%B8%B8%E7%94%A8%E6%8C%87%E4%BB%A4 \r\n");
            strSb.Append($"[作者]：于心\r\n");
            strSb.Append($"本机器人尚未以任何方式进行商业用途，永远开源，不定时更新");
            await SendMessageGroup(eventArgs, strSb.ToString());
        }
        #endregion

        #region 排行榜
        /// <summary>
        /// 排行榜
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask RankList(GroupMessageEventArgs eventArgs)
        {
            var uList = (RequestListUsers()).OrderByDescending(t => t.Rank).Skip(0).Take(15);
            if (uList == null)
                return;
            var strSb = new StringBuilder();
            strSb.Append($"QQ     Nick    Rank\r\n");
            foreach (var item in uList)
            {
                strSb.Append($"{item.QNumber}     {item.NickName}    {item.Rank}\r\n");
            }
            await SendMessageGroup(eventArgs, strSb.ToString());
        }
        #endregion

        #region 特殊事件
        /// <summary>
        /// 特殊事件
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask SpecialEvent(GroupMessageEventArgs eventArgs)
        {
            var uLogs = (_signLogsServices.Query(t => t.CmdType == CmdType.SpecialBonusPoints || t.CmdType == CmdType.SpecialPointsDeducted))?.OrderByDescending(t => t.CreateTime).Take(10);
            if (!uLogs.Any() || uLogs == null)
                return;
            var strSb = new StringBuilder();
            strSb.Append($"时间      事例\r\n");
            foreach (var item in uLogs)
            {
                strSb.Append($"{item.CreateTime:yyyy-MM-dd HH:mm:ss}    {item.LogContent}\r\n");
            }
            await SendMessageGroup(eventArgs, strSb.ToString());
        }
        #endregion

        #region 积分记录
        /// <summary>
        /// LogsRecord
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask LogsRecord(GroupMessageEventArgs eventArgs)
        {
            var uLogs = (_signLogsServices.Query(t => t.Uid.Equals(eventArgs.Sender.Id.ObjToString())))?.OrderByDescending(t => t.CreateTime).Skip(0).Take(15);
            if (!uLogs.Any() || uLogs == null)
                return;
            var strSb = new StringBuilder();
            strSb.Append($"时间   事例    积分\r\n");
            foreach (var item in uLogs)
            {
                strSb.Append($"{item.CreateTime:yyyy-MM-dd HH:mm:ss}    {item.LogContent}    {item.ModifyRank}\r\n");
            }
            await SendMessageGroup(eventArgs, strSb.ToString());
        }
        #endregion

        #region 赠送
        /// <summary>
        /// 赠送
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Giving(GroupMessageEventArgs eventArgs)
        {
            //赠送[CQ:at,qq=503745803] 5
            try
            {
                var giveQ = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[0].ObjectToLong();
                var giveRank = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[1].ObjToString().Replace(" ", "").ObjToInt();
                //校验是否在列表
                var (apiStatus, groupMemberList) = await eventArgs.SourceGroup.SoraApi.GetGroupMemberList(eventArgs.SourceGroup.Id);
                if (groupMemberList.Any(t => t.UserId == giveQ) && giveRank > 0)
                {
                    if (giveQ == eventArgs.Sender.Id)
                        return;
                    var sendUser = RequestUsers(eventArgs.Sender.Id);
                    var giveUser = RequestUsers(giveQ);
                    if (giveUser == null)
                    {
                        await SendMessageGroup(eventArgs, $"赠送失败，赠送对象不存在~", true);
                        return;
                    }
                    if (sendUser.Rank >= giveRank)
                    {
                        sendUser.Rank -= giveRank;
                        giveUser.Rank += giveRank;
                        RequestSignAsync(sendUser, true);
                        RequestSignAsync(giveUser, true);
                        RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.Giving,
                            ModifyRank = giveRank,
                            Uid = giveUser.QNumber,
                            LogContent = $"好友[{sendUser.NickName}]赠送{giveRank}分"
                        });
                        RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.PointsDeducted,
                            ModifyRank = giveRank,
                            Uid = sendUser.QNumber,
                            LogContent = $"赠送好友[{sendUser.NickName}]扣除{giveRank}分"
                        });
                        await SendMessageGroup(eventArgs, $"成功赠送[{giveUser.NickName}]{giveRank}分~情比金坚？", true);
                    }
                }
                else
                    await SendMessageGroup(eventArgs, $"非法操作，请检查积分剩余或者赠送对象是否存在~", true);
            }
            catch (Exception c)
            {
                await SendMessageGroup(eventArgs, c.Message);
            }
        }
        #endregion

        #region 早安
        /// <summary>
        /// 早安
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Morning(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 9)
            {
                var mornLogs = _signLogsServices.Query(t => t.LogContent.Contains("[早安]") && t.LastModifyTime.Day == DateTime.Now.Day && t.LastModifyTime.Year == DateTime.Now.Year && t.LastModifyTime.Month == DateTime.Now.Month) ?? new List<SignLogs>();
                if (mornLogs.Count >= 5)
                    await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}早上好啊~{config.ConfigModel.Tail}", true);
                else
                {
                    if (mornLogs.Any(t => t.Uid.Equals(eventArgs.Sender.Id.ObjToString())))
                    {
                        await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}早起的虫儿被鸟吃~", true);
                        return;
                    }
                    else
                    {
                        var signUser = RequestUsers(eventArgs.Sender.Id);
                        signUser.Rank += 2;
                        signUser.LastModifyTime = DateTime.Now;
                        RequestSignAsync(signUser, true);
                        RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.BonusPoints,
                            LogContent = $"[早安]指令赠送2分",
                            ModifyRank = 2,
                            Uid = eventArgs.Sender.Id.ObjToString()
                        });
                        await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}是第{mornLogs.Count + 1}个说早安的人噢~爱你，奖励2分", true);
                    }
                }
            }
        }
        #endregion

        #region 晚安
        /// <summary>
        /// 晚安
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Night(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            if (DateTime.Now.Hour > 19 && DateTime.Now.Hour < 24)
            {
                var nightLogs = _signLogsServices.Query(t => t.LogContent.Contains("[晚安]") && t.LastModifyTime.Day == DateTime.Now.Day && t.LastModifyTime.Year == DateTime.Now.Year && t.LastModifyTime.Month == DateTime.Now.Month) ?? new List<SignLogs>();
                if (nightLogs.Count >= 5)
                    await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}晚安~美梦美梦zzzzzzzzz", true);
                else
                {
                    if (nightLogs.Any(t => t.Uid.Equals(eventArgs.Sender.Id.ObjToString())))
                    {
                        await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}不是已经睡了嘛？？？？你有问题", true);
                        return;
                    }
                    else
                    {
                        var signUser = RequestUsers(eventArgs.Sender.Id);
                        signUser.Rank += 2;
                        signUser.LastModifyTime = DateTime.Now;
                        RequestSignAsync(signUser, true);
                        RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.BonusPoints,
                            LogContent = $"[晚安]指令赠送2分",
                            ModifyRank = 2,
                            Uid = eventArgs.Sender.Id.ObjToString()
                        });
                        await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}是第{nightLogs.Count + 1}个早睡的人噢~美梦美梦，奖励2分", true);
                    }
                }
            }
        }
        #endregion

        #region 加分
        /// <summary>
        /// 加分
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask BonusPoint(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            //加分[CQ:at,qq=503745803] 5
            try
            {
                if (eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Owner || eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Admin || eventArgs.Sender.Id == 1069430666)
                {
                    var obj = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[0].ObjectToLong();
                    var rank = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[1].ObjToString().Replace(" ", "").ObjToInt();
                    //校验是否在列表
                    var (apiStatus, groupMemberList) = await eventArgs.SourceGroup.SoraApi.GetGroupMemberList(eventArgs.SourceGroup.Id);
                    var objUser = RequestUsers(obj);
                    if (groupMemberList.Any(t => t.UserId == obj) && rank > 0 && objUser is not null)
                    {
                        objUser.Rank += rank;
                        objUser.LastModifyTime = DateTime.Now;
                        RequestSignAsync(objUser, true);
                        RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.Giving,
                            ModifyRank = rank,
                            Uid = obj.ObjToString(),
                            LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]加分{rank}"
                        });
                        await SendMessageGroup(eventArgs, $"{config.ConfigModel.BotName}已经成功为{config.ConfigModel.NickName}[{objUser.NickName}]加分", true);
                    }
                    else
                        await SendMessageGroup(eventArgs, $"[{config.ConfigModel.BotName}]操作失败，检索不到该成员或分数错误", true);
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                await SendMessageGroup(eventArgs, c.Message, true);
            }
        }
        #endregion

        #region 扣分
        /// <summary>
        /// 加分
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask DeductPoint(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            //扣分[CQ:at,qq=503745803] 5
            try
            {
                if (eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Owner || eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Admin || eventArgs.Sender.Id == 1069430666)
                {
                    var obj = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[0].ObjectToLong();
                    var rank = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[1].ObjToString().Replace(" ", "").ObjToInt();
                    //校验是否在列表
                    var (apiStatus, groupMemberList) = await eventArgs.SourceGroup.SoraApi.GetGroupMemberList(eventArgs.SourceGroup.Id);
                    var objUser = RequestUsers(obj);
                    if (groupMemberList.Any(t => t.UserId == obj) && rank > 0 && objUser is not null && objUser.Rank > rank)
                    {
                        objUser.Rank -= rank;
                        objUser.LastModifyTime = DateTime.Now;
                        RequestSignAsync(objUser, true);
                        RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.Giving,
                            ModifyRank = rank,
                            Uid = obj.ObjToString(),
                            LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]扣分{rank}"
                        });
                        await SendMessageGroup(eventArgs, $"{config.ConfigModel.BotName}已经成功扣除{config.ConfigModel.NickName}[{objUser.NickName}]{rank}分", true);
                    }
                    else
                        await SendMessageGroup(eventArgs, $"[{config.ConfigModel.BotName}]操作失败，检索不到该成员或分数错误", true);
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                await SendMessageGroup(eventArgs, c.Message, true);
            }
        }
        #endregion

        #region 全体加分
        /// <summary>
        /// 全体加分
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask AllBonusPoint(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            try
            {
                var rank = eventArgs.Message.RawText.Split("全体加分")[1].ObjToInt();
                if (eventArgs.Message.RawText.Contains("全体加分") && rank > 0)
                {
                    if (eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Owner || eventArgs.Sender.Id == 1069430666)
                    {
                        await SendMessageGroup(eventArgs, $"[{config.ConfigModel.BotName}]正在进行耗时操作，请耐心等待", true);
                        var uList = RequestListUsers();
                        uList.ForEach((uobj) =>
                        {
                            uobj.Rank += rank;
                            uobj.LastModifyTime = DateTime.Now;
                            RequestSignAsync(uobj, true);
                            RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.BonusPoints,
                                LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]为全体成员加{rank}分",
                                ModifyRank = rank,
                                Uid = uobj.QNumber
                            });
                        });
                        RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.SpecialBonusPoints,
                            LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]为全体成员加{rank}分",
                            ModifyRank = rank,
                            Uid = ""
                        });
                        await SendMessageGroup(eventArgs, $"[{config.ConfigModel.BotName}]已成功执行指令", true);
                    }
                }
                else
                    await SendMessageGroup(eventArgs, $"[{config.ConfigModel.BotName}]操作失败，检索不到该指令或分数错误", true);
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                await SendMessageGroup(eventArgs, c.Message, true);
            }
        }
        #endregion

        #region 全体扣分
        /// <summary>
        /// 全体扣分
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask AllDeductPoint(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            try
            {
                var rank = eventArgs.Message.RawText.Split("全体扣分")[1].ObjToInt();
                if (eventArgs.Message.RawText.Contains("全体扣分") && rank > 0)
                {
                    if (eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Owner || eventArgs.Sender.Id == 1069430666)
                    {
                        await SendMessageGroup(eventArgs, $"[{config.ConfigModel.BotName}]正在进行耗时操作，请耐心等待", true);
                        var uList = RequestListUsers();
                        uList.ForEach((uobj) =>
                        {
                            uobj.Rank -= rank;
                            uobj.LastModifyTime = DateTime.Now;
                            RequestSignAsync(uobj, true);
                            RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.PointsDeducted,
                                LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]为全体成员扣{rank}分",
                                ModifyRank = rank,
                                Uid = uobj.QNumber
                            });
                        });
                        RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.SpecialPointsDeducted,
                            LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]为全体成员扣{rank}分",
                            ModifyRank = rank,
                            Uid = ""
                        });
                        await SendMessageGroup(eventArgs, $"[{config.ConfigModel.BotName}]已成功执行指令", true);
                    }
                }
                else
                    await SendMessageGroup(eventArgs, $"[{config.ConfigModel.BotName}]操作失败，检索不到该指令或分数错误", true);
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                await SendMessageGroup(eventArgs, c.Message, true);
            }
        }
        #endregion

        #region RedTea
        /// <summary>
        /// 优质睡眠
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask RedTea(GroupMessageEventArgs eventArgs)
        {
            await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.Sender.Id, 8 * 60 * 60);
            await SendMessageGroup(eventArgs, $"安心睡一觉吧~愿圣光永远守护你", true);
        }
        #endregion

        #region Raffle
        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Raffle(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            if (config.ModuleSwitch.Raffle is not true)
            {
                await SendMessageGroup(eventArgs, $"管理员已关闭[抽奖]功能", true);
                return;
            }
            if (new Random().Next(1, 100) is 66)
            {
                var rank = new Random().Next(5, 10);
                var objUser = RequestUsers(eventArgs.Sender.Id);
                objUser.Rank += rank;
                objUser.LastModifyTime = DateTime.Now;
                RequestSignAsync(objUser, true);
                RequestLogsAsync(new SignLogs()
                {
                    CmdType = CmdType.Giving,
                    ModifyRank = rank,
                    Uid = eventArgs.Sender.Id.ObjToString(),
                    LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]加分{rank}"
                });
                await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}大概就是上天的亲儿子吧，奖励{rank}分", true);
            }
            else
            {
                if (new Random().Next(1, 100) > 70)
                {
                    await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.Sender.Id, 2 * 60);
                    await SendMessageGroup(eventArgs, $"啧，瞅瞅隔壁老王都比你脸白，醒醒吧", true);
                }
                else
                    await SendMessageGroup(eventArgs, $"洗洗睡吧，这种事情不适合你。赶紧照照镜子瞅瞅你这黑脸", true);
            }
        }
        #endregion

        #region Rob
        /// <summary>
        /// Rob
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Rob(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            //打劫[CQ:at,qq=503745803]
            try
            {
                if (config.ModuleSwitch.Rob is not true)
                {
                    await SendMessageGroup(eventArgs, $"管理员已关闭[打劫]功能", true);
                    return;
                }
                var objQ = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[0].ObjectToLong();
                //校验是否在列表
                var (apiStatus, groupMemberList) = await eventArgs.SourceGroup.SoraApi.GetGroupMemberList(eventArgs.SourceGroup.Id);
                if (groupMemberList.Any(t => t.UserId == objQ))
                {
                    var robUser = RequestUsers(eventArgs.Sender.Id);
                    var deRobUser = RequestUsers(objQ);
                    var rank = new Random().Next(2, 5);
                    switch (new Random().Next(0, 10))
                    {
                        case 0:
                            robUser.Rank -= rank;
                            deRobUser.Rank += rank;
                            RequestSignAsync(robUser, true);
                            RequestSignAsync(deRobUser, true);
                            RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.DeRob,
                                ModifyRank = rank,
                                Uid = deRobUser.QNumber,
                                LogContent = $"[抢劫]指令反杀获得{rank}分"
                            });
                            RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.PointsDeducted,
                                ModifyRank = rank,
                                Uid = robUser.QNumber,
                                LogContent = $"[抢劫]指令丢失{rank}分"
                            });
                            await SendMessageGroup(eventArgs, $"路遇[{deRobUser.NickName}]神功初成，{config.ConfigModel.NickName}被一顿暴揍，丢失{rank}分", true);
                            break;
                        case 1:
                            robUser.Rank += rank;
                            deRobUser.Rank -= rank;
                            RequestSignAsync(robUser, true);
                            RequestSignAsync(deRobUser, true);
                            RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.Rob,
                                ModifyRank = rank,
                                Uid = robUser.QNumber,
                                LogContent = $"[抢劫]指令获得{rank}分"
                            });
                            RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.PointsDeducted,
                                ModifyRank = rank,
                                Uid = deRobUser.QNumber,
                                LogContent = $"[抢劫]指令丢失{rank}分"
                            });
                            await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}一顿王八拳之下[{deRobUser.NickName}]抱头求饶，成功抢走{rank}分", true);
                            break;
                        case 2:
                            robUser.Rank -= rank;
                            RequestSignAsync(robUser, true);
                            await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.Sender.Id, 5 * 60);
                            await SendMessageGroup(eventArgs, $"煌煌天威，视我[追命]为何物？\r\n{config.ConfigModel.NickName}锒铛入狱", true);
                            break;
                        default:
                            await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}刚出门就踩到了狗屎，溜了溜了", true);
                            break;
                    }
                }
                else
                    await SendMessageGroup(eventArgs, $"非法操作，请检查该对象是否存在~", true);
            }
            catch (Exception c)
            {
                await SendMessageGroup(eventArgs, c.Message);
            }
        }
        #endregion

        #region Rescur

        /// <summary>
        /// 劫狱 救援
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Rescur(GroupMessageEventArgs eventArgs, UserConfig config)
        {
            //救援[CQ:at,qq=503745803]
            try
            {
                if (config.ModuleSwitch.Rescur is not true)
                {
                    await SendMessageGroup(eventArgs, $"管理员已关闭[救援]功能", true);
                    return;
                }
                var objQ = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[0].ObjectToLong();
                //校验是否在列表
                var (apiStatus, groupMemberList) = await eventArgs.SourceGroup.SoraApi.GetGroupMemberList(eventArgs.SourceGroup.Id);
                if (groupMemberList.Any(t => t.UserId == objQ))
                {
                    switch (new Random().Next(1, 10))
                    {
                        case 1:
                            await eventArgs.SourceGroup.DisableGroupMemberMute(objQ);
                            await SendMessageGroup(eventArgs, $"{config.ConfigModel.NickName}艺高人胆大，成功救出好友", true);
                            break;
                        case 2:
                            await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.SourceGroup.Id, 5 * 60);
                            await SendMessageGroup(eventArgs, $"这天太冷了，{config.ConfigModel.NickName}一声哈欠惊动捕快[冷血]，锒铛入狱", true);
                            break;
                        default:
                            await SendMessageGroup(eventArgs, $"大冬天的，床上它不香吗？睡觉睡觉", true);
                            break;
                    }
                }
                else
                    await SendMessageGroup(eventArgs, $"非法操作，请检查该对象是否存在~", true);
            }
            catch (Exception c)
            {
                await SendMessageGroup(eventArgs, c.Message);
            }
        }
        #endregion

        #region Lian
        /// <summary>
        /// 莲
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Lian(GroupMessageEventArgs eventArgs)
        {
            var chatList = _lianChatServices.Query(t => t.Status == Status.Valid) ?? new List<LianChat>();
            if (chatList == null || chatList.Count <= 0)
            {
                await SendMessageGroup(eventArgs, $"你康康我啊？我就那么不受待见吗");
            }
            else
                await SendMessageGroup(eventArgs, chatList[new Random().Next(chatList.Count)].Content);
        }
        #endregion

        #region 添加关键词
        /// <summary>
        /// 添加关键词
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask AddKeys(GroupMessageEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Owner || eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Admin || eventArgs.Sender.Id == 1069430666)
                {
                    var count = new Regex("#").Matches(eventArgs.Message.RawText).Count;
                    if (count != 2)
                        await SendMessageGroup(eventArgs, $"添加数据密码失败，格式错误\r\n正确格式：添加数据密码#秋水#秋水qwrt", true);
                    else
                    {
                        var keys = eventArgs.Message.RawText.Split("添加数据密码#")[1].Split("#")[0].ObjToString();
                        var name = eventArgs.Message.RawText.Split("添加数据密码#")[1].Split("#")[1].ObjToString();
                        _lianKeyWordsServices.Insert(new LianKeyWords()
                        {
                            Keys = keys,
                            Words = name
                        });
                        await SendMessageGroup(eventArgs, $"添加数据密码[{keys}]成功", true);
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                await SendMessageGroup(eventArgs, c.Message, true);
            }
        }
        #endregion

        #region 添加词库
        /// <summary>
        /// 添加关键词
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask AddThesaurus(GroupMessageEventArgs eventArgs)
        {
            try
            {
                var count = new Regex("#").Matches(eventArgs.Message.RawText).Count;
                if (count != 1)
                    await SendMessageGroup(eventArgs, $"添加关键词失败，格式错误\r\n正确格式：添加词库#海底月是天上月，眼前人是心上人", true);
                else
                {
                    var content = eventArgs.Message.RawText.Split("添加词库#")[1].ObjToString();
                    _lianChatServices.Insert(new LianChat()
                    {
                        Content = content
                    });
                    await SendMessageGroup(eventArgs, $"添加词库成功，下一次可能会出现你的词库噢~", true);
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                await SendMessageGroup(eventArgs, c.Message, true);
            }
        }
        #endregion

        #region 掷骰子
        public async ValueTask RollDice(GroupMessageEventArgs eventArgs)
        {
            var r = new Random().Next(1, 7);
            var msg = new MessageBody()
            {
               CQCodes.CQAt(eventArgs.Sender.Id),
               CQCodes.CQImage($"{Environment.CurrentDirectory}/Images/{ConvertE(r)}.gif")
            };
            await eventArgs.Reply(msg);
        }
        #endregion

        #region 词云
        public async ValueTask WordCloud(GroupMessageEventArgs eventArgs)
        {
            try
            {
                //排除掉图片、带CQ码的
                var speakerLists = _speakerServices.Query(s => s.Uid == eventArgs.Sender.Id && !s.RawText.Contains("image") && !s.RawText.Contains("CQ"));
                if (speakerLists.Any())
                {
                    var builder = string.Join(",", speakerLists.Select(s => s.RawText));
                    //正则过滤所有标点符号
                    builder = Regex.Replace(builder, "[\\s\\p{P}\n\r=<>$>+￥^]", "");
                    var seg = new JiebaSegmenter();
                    var freqs = new Counter<string>(seg.Cut(builder));
                    var filterFreqs = freqs.Count >= 20 ? freqs?.MostCommon(20) : freqs?.MostCommon(freqs.Count - 1);
                    var WordCloudGen = new WordCloudSharp.WordCloud(300, 300, true, fontname: "simsun");
                    var images = WordCloudGen
                        .Draw(filterFreqs.Select(s => s.Key).ToList(), filterFreqs.Select(s => s.Value).ToList());
                    //linux环境下路径需替换为/，windows下的环境为\\
                    var imgName = $"{Environment.CurrentDirectory}/Images/{Guid.NewGuid()}.png";
                    images.Save(imgName, ImageFormat.Png);
                    var msg = new MessageBody
                    {
                        CQCodes.CQAt(eventArgs.Sender.Id),
                        CQCodes.CQImage(imgName)
                    };
                    await eventArgs.Reply(msg);
                    //delete img
                    await Task.Delay(10);
                    File.Delete(imgName);
                }
                else
                    await SendMessageGroup(eventArgs, $"尚不能构造出词云哦 :)", true);
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                await SendMessageGroup(eventArgs, $"尚不能构造出词云哦 :)", true);
            }
        }

        #region 发言榜
        public async ValueTask NonsenseKing(GroupMessageEventArgs eventArgs)
        {
            await SendMessageGroup(eventArgs, "请稍等，正在执行耗时操作");
            var speakerList = _speakerServices
                .Query(q => q.GroupId == eventArgs.SourceGroup.Id && q.CreateTime.Month == DateTime.Now.Month && q.CreateTime.Year == DateTime.Now.Year)
                .GroupBy(g => new { g.Uid })
                .Select(s => new
                {
                    s.Key.Uid,
                    Count = s.Count()
                })
                .OrderByDescending(o => o.Count)
                .Skip(0).Take(15);
            if (speakerList.Any())
            {
                var strSb = new StringBuilder();
                strSb.Append($"----------发言榜----------\r\n");
                foreach (var item in speakerList)
                {
                    var nick = _signUserServices.Query(q => q.QNumber == item.Uid.ObjToString());
                    strSb.Append($"{nick.First().NickName}       {item.Count}条\r\n");
                }
                strSb.Append($"\r\n截止到目前：{DateTime.Now:yyyy-MM-dd}");
                strSb.Append($"\r\n请注意：只计算当前月份");
                await SendMessageGroup(eventArgs, strSb.ToString());
            }
        }
        #endregion


        #endregion

        #region Func
        private static string ConvertE(int r) => r switch
        {
            1 => "one",
            2 => "two",
            3 => "three",
            4 => "four",
            5 => "five",
            6 => "six",
            _ => "random",
        };

        /// <summary>
        /// 触发惩罚机制
        /// </summary>
        private static bool TriggerPunish => DateTime.Now.Millisecond % 2 == 0 && DateTime.Now.Millisecond > 666;

        /// <summary>
        /// 发送群组消息
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="strContent"></param>
        /// <param name="isAt">是否艾特</param>
        /// <returns></returns>
        private static async ValueTask<bool> SendMessageGroup(GroupMessageEventArgs eventArgs, string strContent, bool isAt = false)
        {
            if (isAt)
                await eventArgs.Reply(CQCodes.CQAt(eventArgs.Sender.Id) + strContent);
            else
                await eventArgs.Reply(strContent);
            return false;
        }

        /// <summary>
        /// 日志请求
        /// </summary>
        /// <param name="signLogs"></param>
        /// <param name="isUpdate">是否更新实体</param>
        /// <returns></returns>
        private bool RequestLogsAsync(SignLogs signLogs, bool isUpdate = false)
        {
            if (isUpdate)
            {
                return _signLogsServices.Update(signLogs);
            }
            else
                return _signLogsServices.Insert(signLogs);
        }

        /// <summary>
        /// 积分变更请求
        /// </summary>
        /// <param name="signUser"></param>
        /// <param name="isUpdate">是否更新</param>
        /// <returns></returns>
        private bool RequestSignAsync(SignUser signUser, bool isUpdate = false)
        {
            if (isUpdate)
            {
                return _signUserServices.Update(signUser);
            }
            else
                return _signUserServices.Insert(signUser);
        }


        /// <summary>
        /// 用户请求
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        private SignUser RequestUsers(long uid) => _signUserServices.QueryById(t => t.QNumber.Equals(uid.ObjToString()));

        /// <summary>
        /// 用户列表请求
        /// </summary>
        /// <returns></returns>
        private List<SignUser> RequestListUsers() => _signUserServices.Query(t => t.Status == Status.Valid);

        #endregion
    }
}
