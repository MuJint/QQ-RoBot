using Qiushui.Bot.ChatModule.PcrUtils;
using Qiushui.Lian.Bot.Business;
using Qiushui.Lian.Bot.ChatModule.HsoModule;
using Qiushui.Lian.Bot.ChatModule.LianModule;
using Qiushui.Lian.Bot.Helper;
using Qiushui.Lian.Bot.Helper.ConfigModule;
using Qiushui.Lian.Bot.Models;
using Qiushui.Lian.Bot.Resource;
using Sora.Entities.CQCodes;
using Sora.Enumeration.EventParamsType;
using Sora.EventArgs.SoraEvent;
using Sora.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qiushui.Lian.Bot.ServerInterface
{
    /// <summary>
    /// 群聊事件
    /// </summary>
    internal class GroupMessageEvent
    {
        public static async ValueTask GroupMessageParse(object sender, GroupMessageEventArgs groupMessage)
        {
            //配置文件实例
            Config config = new Config(groupMessage.LoginUid);
            //读取配置文件
            if (!config.LoadUserConfig(out UserConfig userConfig))
            {
                await groupMessage.Reply("读取配置文件(User)时发生错误\r\n请检查配置文件然后重启");
                ConsoleLog.Error("Qiushui机器人管理", "无法读取用户配置文件");
                return;
            }
            if (!IsListenGroup(groupMessage.SourceGroup.Id, userConfig))
                return;
            await IsAIAsync(groupMessage, userConfig);
            await Reread(groupMessage, userConfig);
            await TriggerCute(groupMessage, userConfig);
            await TriggerSpecial(groupMessage, userConfig);

            //聊天关键词
            if (Command.GetKeywordType(groupMessage.Message.RawText, out KeywordCommand keywordCommand))
            {
                ConsoleLog.Info("关键词触发", $"触发关键词[{keywordCommand}]");
                switch (keywordCommand)
                {
                    case KeywordCommand.Hso:
                        if (userConfig.ModuleSwitch.Hso)
                        {
                            HsoHandle hso = new HsoHandle(sender, groupMessage);
                            hso.GetChat();
                        }
                        break;
                    default:
                        if (userConfig.ModuleSwitch.LianBot)
                        {
                            LianHandle lianHandle = new LianHandle(sender, groupMessage);
                            lianHandle.Chat(keywordCommand);
                        }
                        break;
                }
            }
            //正则匹配
            if (Command.GetRegexType(groupMessage.Message.RawText, out RegexCommand regexCommand))
            {
                ConsoleLog.Info("正则触发", $"触发正则匹配[{regexCommand}]");
                switch (regexCommand)
                {
                    case RegexCommand.CheruDecode:
                    case RegexCommand.CheruEncode:
                        //判断模块使能
                        if (userConfig.ModuleSwitch.Cheru)
                        {
                            CheruHandle cheru = new CheruHandle(sender, groupMessage);
                            cheru.GetChat(regexCommand);
                        }
                        break;
                    //调试
                    case RegexCommand.Debug:
                        if (groupMessage.SenderInfo.Role == MemberRoleType.Member)
                        {
                            ConsoleLog.Warning("Auth Warning", $"成员[{groupMessage.Sender.Id}]正尝试执行调试指令");
                        }
                        else
                        {
                        }
                        break;
                    //将其他的的全部交给娱乐模块处理
                    default:
                        if (userConfig.ModuleSwitch.LianBot)
                        {
                            LianHandle lianHandle = new LianHandle(sender, groupMessage);
                            lianHandle.Chat(keywordCommand);
                        }
                        break;
                }
            }
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
        private static async ValueTask IsAIAsync(GroupMessageEventArgs eventArgs, UserConfig userConfig)
        {
            //[CQ:at,qq=503745803] 你好啊
            try
            {
                var at = $"[CQ:at,qq={eventArgs.LoginUid}]";
                if (eventArgs.Message.RawText.Contains(at) && userConfig.ModuleSwitch.IsAI)
                {
                    var service = new DealInstruction(userConfig);
                    var json = await service.RequestAi(eventArgs.Message.RawText.Replace(at, "").Replace(" ", ""));
                    await eventArgs.Reply(CQCode.CQAt(eventArgs.Sender.Id), json.ObjectToGBK());
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
        private static async ValueTask TriggerCute(GroupMessageEventArgs eventArgs, UserConfig userConfig)
        {
            var service = new DealInstruction(userConfig);
            if (new Random().Next(1, 100) is 66)
            {
                var user = await service.RequestUsers(eventArgs.SenderInfo.UserId);
                var rank = new Random().Next(1, 10);
                if (user == null)
                {
                    await eventArgs.Reply(CQCode.CQAt(eventArgs.SenderInfo.UserId), $"未找到{userConfig.ConfigModel.NickName}任何记录，奖励下发失败~");
                }
                else
                {
                    user.Rank += rank;
                    user.LastModifyTime = DateTime.Now;
                    await service.RequestSignAsync(user, true);
                    await service.RequestLogsAsync(new SignLogs()
                    {
                        CmdType = CmdType.BonusPoints,
                        LogContent = $"[可爱]指令赠送{rank}分",
                        ModifyRank = rank,
                        Uid = eventArgs.SenderInfo.UserId.ObjToString()
                    });
                    await eventArgs.Reply(CQCode.CQAt(eventArgs.SenderInfo.UserId), $"看{userConfig.ConfigModel.NickName}这么可爱，就奖励{userConfig.ConfigModel.NickName}{rank}分~");
                }
            }
        }

        /// <summary>
        /// 特殊事件
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="userConfig"></param>
        /// <returns></returns>
        private static async ValueTask TriggerSpecial(GroupMessageEventArgs eventArgs, UserConfig userConfig)
        {
            var deTrigger = new Random().Next(1, 5000) is 666;
            var trigger = new Random().Next(1, 5000) is 444;
            var rank = new Random().Next(7, 12);
            var service = new DealInstruction(userConfig);
            var strSb = new StringBuilder();
            if (deTrigger)
            {
                var uList = await service.RequestListUsers();
                uList.ForEach(async (item) =>
                {
                    item.Rank -= rank;
                    item.LastModifyTime = DateTime.Now;
                    await service.RequestSignAsync(item, true);
                });
                strSb.Append($"[江湖传言]\r\n");
                strSb.Append($"恭喜{userConfig.ConfigModel.NickName}[{eventArgs.SenderInfo.Nick}]通过挖宝拾取道具：陨焰之盒 × 1\r\n");
                strSb.Append($"就你有手？奖励所有{userConfig.ConfigModel.NickName}负{rank}分");
                await service.RequestLogsAsync(new SignLogs()
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
                var uList = await service.RequestListUsers();
                uList.ForEach(async (item) =>
                {
                    item.Rank += rank;
                    item.LastModifyTime = DateTime.Now;
                    await service.RequestSignAsync(item, true);
                });
                strSb.Append($"[江湖传言]\r\n");
                strSb.Append($"恭喜{userConfig.ConfigModel.NickName}[{userConfig.ConfigModel.BotName}]通过挖宝拾取道具：陨焰之盒 × 1\r\n");
                strSb.Append($"普天同庆！！！奖励所有{userConfig.ConfigModel.NickName}{rank}分");
                await service.RequestLogsAsync(new SignLogs()
                {
                    CmdType = CmdType.SpecialBonusPoints,
                    LogContent = $"普天同庆！！！奖励所有{userConfig.ConfigModel.NickName}{rank}分",
                    Uid = eventArgs.LoginUid.ObjToString(),
                    ModifyRank = rank
                });
                await eventArgs.Reply(strSb.ToString());
            }
        }
        #endregion
    }
}
