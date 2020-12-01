using Qiushui.Lian.Bot.Framework.IServices;
using Qiushui.Lian.Bot.Framework.Services;
using Qiushui.Lian.Bot.Helper;
using Qiushui.Lian.Bot.Helper.ConfigModule;
using Qiushui.Lian.Bot.Models;
using Sora.Entities.CQCodes;
using Sora.EventArgs.SoraEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qiushui.Lian.Bot.Business
{
    /// <summary>
    /// 指令处理
    /// </summary>
    internal class DealInstruction
    {
        readonly int randRank = new Random().Next(1, 10);
        readonly ISignUserServices signUserServices = new SignUserServices();
        readonly ISignLogsServices signLogsServices = new SignLogsServices();
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
        /// 签到请求
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
                if (isSign.LastModifyTime.DayOfYear == DateTime.Now.DayOfYear)
                {
                    if (TriggerPunish)
                    {
                        await eventArgs.SourceGroup.EnableGroupMemberMute(eventArgs.Sender.Id, 3 * 60);
                        await SendMessageGroup(eventArgs, $"惊不惊喜，意不意外？", true);
                    }
                    else
                        await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}不要重复签到鸭，不然{_config.ConfigModel.BotName}会打飞你的头嗷~", true);
                }
                else
                {
                    isSign.LastModifyTime = DateTime.Now;
                    isSign.Rank += randRank;
                    isSign.NickName = eventArgs.SenderInfo.Nick ?? "";
                    await RequestSignAsync(isSign, true);
                    await RequestLogsAsync(new SignLogs()
                    {
                        LogContent = $"签到成功，赠送积分{randRank}",
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
                    LogContent = $"签到成功，赠送积分{randRank}",
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
                await SendMessageGroup(eventArgs, $"{_config.ConfigModel.NickName}当前有{isSign.Rank}分，继续努力吧~最后一次积分变更{isSign.LastModifyTime:yyyy-MM-dd HH:mm:ss}", true);
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
                        LogContent = $"【分来指令送分】{randRank}分",
                        ModifyRank = randRank,
                        Uid = eventArgs.Sender.Id.ObjToString()
                    });
                    await SendMessageGroup(eventArgs, $"看{_config.ConfigModel.NickName}这么可爱，就送{_config.ConfigModel.NickName}{randRank}分吧~", true);
                }
            }
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
            strSb.Append($"[使用说明]：https://www.changqingmao.com/Article/43.html \r\n");
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
            var strSb = new StringBuilder();
            strSb.Append($"QQ     Nick    Rank\r\n");
            foreach (var item in uList)
            {
                strSb.Append($"{item.QNumber}     {item.NickName}    {item.Rank}\r\n");
            }
            await SendMessageGroup(eventArgs, strSb.ToString());
        }
        #endregion

    }
}
