using Qiushui.Bot.Framework.IServices;
using Qiushui.Bot.Framework.Services;
using Qiushui.Bot.Helper;
using Qiushui.Bot.Helper.ConfigModule;
using Qiushui.Bot.Models;
using Sora.Entities.CQCodes;
using Sora.EventArgs.SoraEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Qiushui.Bot.Business
{
    /// <summary>
    /// 指令处理
    /// </summary>
    internal class DealInstruction
    {
        readonly int randRank = new Random().Next(2, 5);
        readonly ISignUserServices signUserServices = new SignUserServices();
        readonly ISignLogsServices signLogsServices = new SignLogsServices();
        readonly ILianChatServices lianChatServices = new LianChatServices();
        readonly ILianKeyWordsServices lianKeyWordsServices = new LianKeyWordsServices();
        readonly UserConfig _config;
        public DealInstruction(UserConfig config)
        {
            _config = config;
        }

        #region Func
        /// <summary>
        /// 触发惩罚机制
        /// </summary>
        internal static bool TriggerPunish => DateTime.Now.Millisecond % 2 == 0 && DateTime.Now.Millisecond > 666;

        /// <summary>
        /// 发送群组消息
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="strContent"></param>
        /// <param name="isAt">是否艾特</param>
        /// <returns></returns>
        internal static async ValueTask<bool> SendMessageGroup(GroupMessageEventArgs eventArgs, string strContent, bool isAt = false)
        {
            if (isAt)
                await eventArgs.Reply(CQCode.CQAt(eventArgs.Sender.Id), strContent);
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
        internal async ValueTask<bool> RequestLogsAsync(SignLogs signLogs, bool isUpdate = false)
        {
            if (isUpdate)
            {
                return await signLogsServices.Update(signLogs);
            }
            else
                return await signLogsServices.Insert(signLogs);
        }

        /// <summary>
        /// 积分变更请求
        /// </summary>
        /// <param name="signUser"></param>
        /// <param name="isUpdate">是否更新</param>
        /// <returns></returns>
        internal async ValueTask<bool> RequestSignAsync(SignUser signUser, bool isUpdate = false)
        {
            if (isUpdate)
            {
                return await signUserServices.Update(signUser);
            }
            else
                return await signUserServices.Insert(signUser);
        }

        /// <summary>
        /// 人工智障请求
        /// 未涉及到机器学习
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal async ValueTask<string> RequestAi(string str) => await HttpHelper.HttpGetAsync($"{_config.ConfigModel.AiPath}/{str}");

        /// <summary>
        /// 用户请求
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        internal async ValueTask<SignUser> RequestUsers(long uid) => await signUserServices.QueryById(t => t.QNumber.Equals(uid.ObjToString()));

        /// <summary>
        /// 用户列表请求
        /// </summary>
        /// <returns></returns>
        internal async ValueTask<List<SignUser>> RequestListUsers() => await signUserServices.Query(t => t.Status == Status.Valid);
        #endregion

        #region 签到方法
        /// <summary>
        /// 签到方法
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask SignIn(GroupMessageEventArgs eventArgs)
        {
            var isSign = await RequestUsers(eventArgs.SenderInfo.UserId);
            if (isSign != null)
            {
                var signLog = (await signLogsServices.Query(t => t.CmdType == CmdType.SignIn && t.Uid.Equals(eventArgs.SenderInfo.UserId.ObjToString()))).OrderByDescending(t => t.LastModifyTime)?.FirstOrDefault() ?? new SignLogs();
                if (signLog.LastModifyTime.DayOfYear == DateTime.Now.DayOfYear && signLog.ModifyRank > 0)
                {
                    if (TriggerPunish)
                    {
                        await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.Sender.Id, 3 * 60);
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}不要重复签到yo~惊不惊喜，意不意外？", true);
                    }
                    else
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}不要重复签到鸭，不然{_config.ConfigModel.BotName}会打飞你的头嗷~", true);
                }
                else
                {
                    isSign.LastModifyTime = DateTime.Now;
                    isSign.Rank += randRank;
                    isSign.GroupId = eventArgs.SourceGroup.Id.ObjToString();
                    isSign.NickName = eventArgs.SenderInfo.Nick ?? "";
                    await RequestSignAsync(isSign, true);
                    await RequestLogsAsync(new SignLogs()
                    {
                        LogContent = $"[签到]成功赠送积分{randRank}",
                        Uid = eventArgs.SenderInfo.UserId.ObjToString(),
                        ModifyRank = randRank
                    });
                    await SendMessageGroup(eventArgs, $"恭喜{_config.ConfigModel.NickName}签到成功，奖励{randRank}分{_config.ConfigModel.Tail}", true);
                }
            }
            else
            {
                //初次无记录
                await RequestSignAsync(new SignUser()
                {
                    GroupId = eventArgs.SourceGroup.Id.ObjToString(),
                    NickName = eventArgs.SenderInfo.Nick,
                    QNumber = eventArgs.SenderInfo.UserId.ObjToString(),
                    Rank = randRank
                });
                await RequestLogsAsync(new SignLogs()
                {
                    LogContent = $"[签到]成功赠送积分{randRank}",
                    Uid = eventArgs.SenderInfo.UserId.ObjToString(),
                    ModifyRank = randRank
                });
                await SendMessageGroup(eventArgs, $"恭喜{_config.ConfigModel.NickName}签到成功，奖励{randRank}分{_config.ConfigModel.Tail}", true);
            }
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask SearchRank(GroupMessageEventArgs eventArgs)
        {
            var isSign = await signUserServices.QueryById(t => t.QNumber.Equals(eventArgs.SenderInfo.UserId.ObjToString()));
            if (isSign != null)
                await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}当前有{isSign.Rank}分，继续努力吧~", true);
            else
                await SendMessageGroup(eventArgs, $"没有找到任何记录噢~请先对{_config.ConfigModel.BotName}说签到吧", true);
        }
        #endregion

        #region 分来
        /// <summary>
        /// 分来
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Fenlai(GroupMessageEventArgs eventArgs)
        {
            if (new Random().Next(1, 100) is 6)
            {
                var isSign = await signUserServices.QueryById(t => t.QNumber.Equals(eventArgs.SenderInfo.UserId.ObjToString()));
                if (isSign != null)
                    await SendMessageGroup(eventArgs, $"没有找到任何记录噢~请先对{_config.ConfigModel.BotName}说签到吧", true);
                else
                {
                    isSign.Rank += randRank;
                    isSign.LastModifyTime = DateTime.Now;
                    await RequestSignAsync(isSign, true);
                    await RequestLogsAsync(new SignLogs()
                    {
                        CmdType = CmdType.BonusPoints,
                        LogContent = $"[分来]指令赠送{randRank}分",
                        ModifyRank = randRank,
                        Uid = eventArgs.Sender.Id.ObjToString()
                    });
                    await SendMessageGroup(eventArgs, $"看{_config.ConfigModel.NickName}这么可爱，就送{_config.ConfigModel.NickName}{randRank}分吧~", true);
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
        public async ValueTask Skill(GroupMessageEventArgs eventArgs)
        {
            var strSb = new StringBuilder();
            strSb.Append($"[当前机器人]：{_config.ConfigModel.BotName}\r\n");
            strSb.Append($"[开源地址]：https://github.com/MuJint/Qiushui-Bot.git \r\n");
            strSb.Append($"[使用说明]：https://www.qiubb.com/article/mtuxmty0nzu2mdu2mtu2qqm= \r\n");
            strSb.Append($"[作者]：于心\r\n");
            strSb.Append($"出于良心考虑，我不建议您通过任何方式进行商业用途，永远开源，不定时更新。能帮忙点个小星星最好~爱你们");
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
            var uList = (await RequestListUsers()).OrderByDescending(t => t.Rank).Skip(0).Take(15);
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
            var uLogs = (await signLogsServices.Query(t => t.CmdType == CmdType.SpecialBonusPoints || t.CmdType == CmdType.SpecialPointsDeducted))?.OrderByDescending(t => t.CreateTime).Take(10);
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
            var uLogs = (await signLogsServices.Query(t => t.Uid.Equals(eventArgs.Sender.Id.ObjToString())))?.OrderByDescending(t => t.CreateTime).Skip(0).Take(15);
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
                    var sendUser = await RequestUsers(eventArgs.Sender.Id);
                    var giveUser = await RequestUsers(giveQ);
                    if (giveUser == null)
                    {
                        await SendMessageGroup(eventArgs, $"赠送失败，赠送对象不存在~", true);
                        return;
                    }
                    if (sendUser.Rank >= giveRank)
                    {
                        sendUser.Rank -= giveRank;
                        giveUser.Rank += giveRank;
                        await RequestSignAsync(sendUser, true);
                        await RequestSignAsync(giveUser, true);
                        await RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.Giving,
                            ModifyRank = giveRank,
                            Uid = giveUser.QNumber,
                            LogContent = $"好友[{sendUser.NickName}]赠送{giveRank}分"
                        });
                        await RequestLogsAsync(new SignLogs()
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
        public async ValueTask Morning(GroupMessageEventArgs eventArgs)
        {
            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 9)
            {
                var mornLogs = await signLogsServices.Query(t => t.LogContent.Contains("[早安]") && t.LastModifyTime.Day == DateTime.Now.Day && t.LastModifyTime.Year == DateTime.Now.Year && t.LastModifyTime.Month == DateTime.Now.Month) ?? new List<SignLogs>();
                if (mornLogs.Count >= 5)
                    await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}早上好啊~{_config.ConfigModel.Tail}", true);
                else
                {
                    if (mornLogs.Any(t => t.Uid.Equals(eventArgs.Sender.Id.ObjToString())))
                    {
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}早起的虫儿被鸟吃~", true);
                        return;
                    }
                    else
                    {
                        var signUser = await RequestUsers(eventArgs.Sender.Id);
                        signUser.Rank += 2;
                        signUser.LastModifyTime = DateTime.Now;
                        await RequestSignAsync(signUser, true);
                        await RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.BonusPoints,
                            LogContent = $"[早安]指令赠送2分",
                            ModifyRank = 2,
                            Uid = eventArgs.Sender.Id.ObjToString()
                        });
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}是第{mornLogs.Count + 1}个说早安的人噢~爱你，奖励2分", true);
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
        public async ValueTask Night(GroupMessageEventArgs eventArgs)
        {
            if (DateTime.Now.Hour > 19 && DateTime.Now.Hour < 24)
            {
                var nightLogs = await signLogsServices.Query(t => t.LogContent.Contains("[晚安]") && t.LastModifyTime.Day == DateTime.Now.Day && t.LastModifyTime.Year == DateTime.Now.Year && t.LastModifyTime.Month == DateTime.Now.Month) ?? new List<SignLogs>();
                if (nightLogs.Count >= 5)
                    await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}晚安~美梦美梦zzzzzzzzz", true);
                else
                {
                    if (nightLogs.Any(t => t.Uid.Equals(eventArgs.Sender.Id.ObjToString())))
                    {
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}不是已经睡了嘛？？？？你有问题", true);
                        return;
                    }
                    else
                    {
                        var signUser = await RequestUsers(eventArgs.Sender.Id);
                        signUser.Rank += 2;
                        signUser.LastModifyTime = DateTime.Now;
                        await RequestSignAsync(signUser, true);
                        await RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.BonusPoints,
                            LogContent = $"[晚安]指令赠送2分",
                            ModifyRank = 2,
                            Uid = eventArgs.Sender.Id.ObjToString()
                        });
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}是第{nightLogs.Count + 1}个早睡的人噢~美梦美梦，奖励2分", true);
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
        public async ValueTask BonusPoint(GroupMessageEventArgs eventArgs)
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
                    if (groupMemberList.Any(t => t.UserId == obj) && rank > 0)
                    {
                        var objUser = await RequestUsers(obj);
                        objUser.Rank += rank;
                        objUser.LastModifyTime = DateTime.Now;
                        await RequestSignAsync(objUser, true);
                        await RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.Giving,
                            ModifyRank = rank,
                            Uid = obj.ObjToString(),
                            LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]加分{rank}"
                        });
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.BotName}已经成功为{_config.ConfigModel.BotName}[{objUser.NickName}]加分", true);
                    }
                    else
                        await SendMessageGroup(eventArgs, $"[{_config.ConfigModel.BotName}]操作失败，检索不到该成员或分数错误", true);
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
        public async ValueTask DeductPoint(GroupMessageEventArgs eventArgs)
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
                    if (groupMemberList.Any(t => t.UserId == obj) && rank > 0)
                    {
                        var objUser = await RequestUsers(obj);
                        objUser.Rank -= rank;
                        objUser.LastModifyTime = DateTime.Now;
                        await RequestSignAsync(objUser, true);
                        await RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.Giving,
                            ModifyRank = rank,
                            Uid = obj.ObjToString(),
                            LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]扣分{rank}"
                        });
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.BotName}已经成功扣除{_config.ConfigModel.BotName}[{objUser.NickName}]{rank}分", true);
                    }
                    else
                        await SendMessageGroup(eventArgs, $"[{_config.ConfigModel.BotName}]操作失败，检索不到该成员或分数错误", true);
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
        public async ValueTask AllBonusPoint(GroupMessageEventArgs eventArgs)
        {
            try
            {
                var rank = eventArgs.Message.RawText.Split("全体加分")[1].ObjToInt();
                if (eventArgs.Message.RawText.Contains("全体加分") && rank > 0)
                {
                    if (eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Owner || eventArgs.Sender.Id == 1069430666)
                    {
                        await SendMessageGroup(eventArgs, $"[{_config.ConfigModel.BotName}]正在进行操作，请耐心等待", true);
                        var uList = await RequestListUsers();
                        uList.ForEach(async (uobj) =>
                       {
                           uobj.Rank += rank;
                           uobj.LastModifyTime = DateTime.Now;
                           await RequestSignAsync(uobj, true);
                       });
                        await RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.SpecialBonusPoints,
                            LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]为全体成员加{rank}分",
                            ModifyRank = rank,
                            Uid = ""
                        });
                    }
                }
                else
                    await SendMessageGroup(eventArgs, $"[{_config.ConfigModel.BotName}]操作失败，检索不到该指令或分数错误", true);
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
        public async ValueTask AllDeductPoint(GroupMessageEventArgs eventArgs)
        {
            try
            {
                var rank = eventArgs.Message.RawText.Split("全体扣分")[1].ObjToInt();
                if (eventArgs.Message.RawText.Contains("全体扣分") && rank > 0)
                {
                    if (eventArgs.SenderInfo.Role == Sora.Enumeration.EventParamsType.MemberRoleType.Owner || eventArgs.Sender.Id == 1069430666)
                    {
                        await SendMessageGroup(eventArgs, $"[{_config.ConfigModel.BotName}]正在进行操作，请耐心等待", true);
                        var uList = await RequestListUsers();
                        uList.ForEach(async (uobj) =>
                        {
                            uobj.Rank -= rank;
                            uobj.LastModifyTime = DateTime.Now;
                            await RequestSignAsync(uobj, true);
                        });
                        await RequestLogsAsync(new SignLogs()
                        {
                            CmdType = CmdType.SpecialBonusPoints,
                            LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]为全体成员扣{rank}分",
                            ModifyRank = rank,
                            Uid = ""
                        });
                    }
                }
                else
                    await SendMessageGroup(eventArgs, $"[{_config.ConfigModel.BotName}]操作失败，检索不到该指令或分数错误", true);
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
            await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.Sender.Id, 12 * 60 * 60);
            await SendMessageGroup(eventArgs, $"安心睡一觉吧~愿圣光永远守护你", true);
        }
        #endregion

        #region Raffle
        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async ValueTask Raffle(GroupMessageEventArgs eventArgs)
        {
            if (_config.ModuleSwitch.Raffle is not true)
            {
                await SendMessageGroup(eventArgs, $"管理员已关闭[抽奖]功能", true);
                return;
            }
            if (new Random().Next(1, 100) is 66)
            {
                var rank = new Random().Next(5, 10);
                var objUser = await RequestUsers(eventArgs.Sender.Id);
                objUser.Rank += rank;
                objUser.LastModifyTime = DateTime.Now;
                await RequestSignAsync(objUser, true);
                await RequestLogsAsync(new SignLogs()
                {
                    CmdType = CmdType.Giving,
                    ModifyRank = rank,
                    Uid = eventArgs.Sender.Id.ObjToString(),
                    LogContent = $"管理员[{eventArgs.SenderInfo.Nick}]加分{rank}"
                });
                await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}大概就是上天的亲儿子吧，奖励{rank}分", true);
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
        public async ValueTask Rob(GroupMessageEventArgs eventArgs)
        {
            //打劫[CQ:at,qq=503745803]
            try
            {
                if (_config.ModuleSwitch.Rob is not true)
                {
                    await SendMessageGroup(eventArgs, $"管理员已关闭[打劫]功能", true);
                    return;
                }
                var objQ = eventArgs.Message.RawText.Split("[CQ:at,qq=")[1].Split("]")[0].ObjectToLong();
                //校验是否在列表
                var (apiStatus, groupMemberList) = await eventArgs.SourceGroup.SoraApi.GetGroupMemberList(eventArgs.SourceGroup.Id);
                if (groupMemberList.Any(t => t.UserId == objQ))
                {
                    var robUser = await RequestUsers(eventArgs.Sender.Id);
                    var deRobUser = await RequestUsers(objQ);
                    var rank = new Random().Next(2, 5);
                    switch (new Random().Next(0, 10))
                    {
                        case 0:
                            robUser.Rank -= rank;
                            deRobUser.Rank += rank;
                            await RequestSignAsync(robUser, true);
                            await RequestSignAsync(deRobUser, true);
                            await RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.DeRob,
                                ModifyRank = rank,
                                Uid = deRobUser.QNumber,
                                LogContent = $"[抢劫]指令反杀获得{rank}分"
                            });
                            await RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.PointsDeducted,
                                ModifyRank = rank,
                                Uid = robUser.QNumber,
                                LogContent = $"[抢劫]指令丢失{rank}分"
                            });
                            await SendMessageGroup(eventArgs, $"路遇[{deRobUser.NickName}]神功初成，{_config.ConfigModel.NickName}被一顿暴揍，丢失{rank}分", true);
                            break;
                        case 1:
                            robUser.Rank += rank;
                            deRobUser.Rank -= rank;
                            await RequestSignAsync(robUser, true);
                            await RequestSignAsync(deRobUser, true);
                            await RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.Rob,
                                ModifyRank = rank,
                                Uid = robUser.QNumber,
                                LogContent = $"[抢劫]指令获得{rank}分"
                            });
                            await RequestLogsAsync(new SignLogs()
                            {
                                CmdType = CmdType.PointsDeducted,
                                ModifyRank = rank,
                                Uid = deRobUser.QNumber,
                                LogContent = $"[抢劫]指令丢失{rank}分"
                            });
                            await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}一顿王八拳之下[{deRobUser.NickName}]抱头求饶，成功抢走{rank}分", true);
                            break;
                        case 2:
                            robUser.Rank -= rank;
                            await RequestSignAsync(robUser, true);
                            await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.Sender.Id, 5 * 60);
                            await SendMessageGroup(eventArgs, $"煌煌天威，视我[追命]为何物？\r\n{_config.ConfigModel.NickName}锒铛入狱", true);
                            break;
                        default:
                            await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}刚出门就踩到了狗屎，溜了溜了", true);
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
        public async ValueTask Rescur(GroupMessageEventArgs eventArgs)
        {
            //救援[CQ:at,qq=503745803]
            try
            {
                if (_config.ModuleSwitch.Rescur is not true)
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
                            await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}艺高人胆大，成功救出好友", true);
                            break;
                        case 2:
                            await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.SourceGroup.Id, 5 * 60);
                            await SendMessageGroup(eventArgs, $"这天太冷了，{_config.ConfigModel.NickName}一声哈欠惊动捕快[冷血]，锒铛入狱", true);
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
            var chatList = await lianChatServices.Query(t => t.Status == Status.Valid) ?? new List<LianChat>();
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
                        await lianKeyWordsServices.Insert(new LianKeyWords()
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
                    await lianChatServices.Insert(new LianChat()
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
    }
}
