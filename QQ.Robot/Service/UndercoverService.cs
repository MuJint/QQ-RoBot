using Robot.Common;
using Robot.Framework.Interface;
using Robot.Framework.Models;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    /// <summary>
    /// UndercoverService
    /// <para>谁是卧底实现</para>
    /// </summary>
    public class UndercoverService : IUndercoverInterface
    {
        readonly IUndercoverServices _undercoverServices;
        readonly IUndercoverUserServices _undercoverUserServices;
        readonly IUndercoverLexiconServices _undercoverLexiconServices;

        public UndercoverService(IUndercoverServices undercoverServices,
            IUndercoverUserServices undercoverUserServices,
            IUndercoverLexiconServices undercoverLexiconServices)
        {
            _undercoverServices = undercoverServices;
            _undercoverUserServices = undercoverUserServices;
            _undercoverLexiconServices = undercoverLexiconServices;
        }

        /// <summary>
        /// <seealso cref="IUndercoverInterface.JoinGame(GroupMessageEventArgs)"/>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask JoinGame(GroupMessageEventArgs eventArgs)
        {
            //jrwd1
            _ = int.TryParse(eventArgs.Message.RawText.Split("jrwd")[1], out var roomId);
            if (roomId <= 0)
                await SendMessageGroup(eventArgs, "加入房间失败。请输入正确的房间号", true, true);
            //查找房间
            var room = _undercoverServices.Query(w => w.IsStart == false && w.GroupId == eventArgs.SourceGroup.Id && w.ID == roomId).FirstOrDefault();
            if (room is null)
                await SendMessageGroup(eventArgs, "加入房间失败。未找到房间信息", true, true);
            //验证用户
            var undercoverUsers = _undercoverUserServices.Query(w => w.Uid == eventArgs.Sender.Id);
            var roomIds = undercoverUsers.Select(s => s.RoomId).ToList();
            var rooms = _undercoverServices.Query(w => roomIds.Contains(w.ID) && w.IsStart == false && w.Status == Status.Valid);
            if (rooms.Count >= 1)
                await SendMessageGroup(eventArgs, "您已加入其它房间。", true, true);
            //加入房间
            _undercoverUserServices.Insert(new UndercoverUser()
            {
                RoomId = roomId,
                Uid = eventArgs.Sender.Id,
                Nick = eventArgs.SenderInfo.Nick
            });
            //校验是否可以自动开始游戏
            var users = _undercoverUserServices.Query(w => w.RoomId == roomId);
            if (users.Count >= 2)
            {
                //
                await SendMessageGroup(eventArgs, $"您已加入【{roomId}】号房间，即将开始游戏", true);
                await StartGame(eventArgs, roomId);
            }
            else
                await SendMessageGroup(eventArgs, $"您已加入【{roomId}】号房间，当前已有{users.Count}人", true);
        }

        /// <summary>
        /// <seealso cref="IUndercoverInterface.StartGame(GroupMessageEventArgs,int)"/>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask StartGame(GroupMessageEventArgs eventArgs, int roomId)
        {
            var room = _undercoverServices.Query(w => w.ID == roomId).FirstOrDefault();
            var users = _undercoverUserServices.Query(w => w.RoomId == roomId);
            var uids = users.Select(s => s.Uid).ToList();
            room.IsStart = true;
            _undercoverServices.Update(room);

            //词库
            //var lexicons = _undercoverLexiconServices.Query(w => !uids.Contains(w.Uid));
            var lexicons = _undercoverLexiconServices.Query(w => w.Status == Status.Valid);
            var lexicon = lexicons[new Random().Next(0, lexicons.Count)];
            var undercoverUser = users[new Random().Next(0, users.Count)];

            var strBuilder = new StringBuilder();
            strBuilder.Append($"【{roomId}】号房玩家列表：");
            var index = 1;
            foreach (var user in users)
            {
                if (user == undercoverUser)
                {
                    //卧底
                    await SendTemporaryMessage(eventArgs, user.Uid, room.GroupId, $"您的词语是：【{lexicon.UndercoverWord}】");
                    user.IsUndercover = true;
                    _undercoverUserServices.Update(user);
                }
                else
                    await SendTemporaryMessage(eventArgs, user.Uid, room.GroupId, $"您的词语是：【{lexicon.Word}】");
                strBuilder.Append($"{++index}号【{user.Nick}】\r\n");
            }
            //发送群组消息
            await SendMessageGroup(eventArgs, strBuilder.ToString());
            await Task.Delay(1000 * 2);
            await SendMessageGroup(eventArgs, new MessageBody()
            {
                SoraSegment.Text("请"),
                SoraSegment.At(users[0].Uid),
                SoraSegment.Text("开始发言"),
                SoraSegment.Text("\r\n每轮发言完毕，请房主"),
                SoraSegment.At(room.CreateUid),
                SoraSegment.Text("发起投票。\r\n发起投票命令【fqtp】，投票命令【tpy1】对一号玩家投票"),
            });
        }

        /// <summary>
        /// <seealso cref="IUndercoverInterface.StopGame(GroupMessageEventArgs,bool)"/>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="undercoverWin"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask StopGame(GroupMessageEventArgs eventArgs, bool undercoverWin = false)
        {
            if(undercoverWin)
                await SendMessageGroup(eventArgs, $"本轮游戏卧底胜利😥😥");
            else
                await SendMessageGroup(eventArgs, $"本轮游戏平民胜利😊✌️");
            var room = _undercoverServices.Query(w => w.CreateUid == eventArgs.Sender.Id && w.IsStart).FirstOrDefault();
            room.Status = Status.InValid;
            room.IsStart = false;
            _undercoverServices.Update(room);
            var lexicon = _undercoverLexiconServices.Query(w => w.ID == room.UndercoverLexiconId).FirstOrDefault();
            await SendMessageGroup(eventArgs, $"感谢【{lexicon.Nick}】提供的词库。本轮关键词【{lexicon.Word}】，卧底关键词【{lexicon.UndercoverWord}】");

            //移除投票结果
            GlobalSettings.TpResult.RemoveAll(w => w.RoomId == room.ID);
        }

        /// <summary>
        /// <seealso cref="IUndercoverInterface.Undercover(GroupMessageEventArgs)"/>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask Undercover(GroupMessageEventArgs eventArgs)
        {
            //查询当前是否有未开始的房间
            var rooms = _undercoverServices.Query(w => w.GroupId == eventArgs.SourceGroup.Id && w.IsStart == false);
            var strContent = string.Empty;
            strContent = $"谁是卧底：\r\n标准七人局：一人卧底，六人平民。每轮按顺序发言票推出一人，平票则下一轮发言\r\n若卧底被票出，平民胜利。若场上剩余三人包含卧底，卧底胜利\r\n输入：【sswd】可查看当前群聊房间列表\r\n输入：【jsfj1】可解散1号房间\r\n输入：【jrwd1】加入1号房间\r\n投票带上前缀【tpy】，例如【tpy1】\r\n可私聊机器人：（前者平民关键词，后者卧底关键词）添加谁是卧底#桃子#梨子\r\n";
            await SendMessageGroup(eventArgs, strContent);
            if (rooms.Count >= 5)
                await SendMessageGroup(eventArgs, $"当前群聊未开始房间已经存在五个，不可创建");
            else
            {
                //词库
                var undercoverLexicons = _undercoverLexiconServices.Query(w => w.Status == Status.Valid);
                if (undercoverLexicons.Count <= 0)
                    await InitalizeLexicon();
                undercoverLexicons = _undercoverLexiconServices.Query(w => w.Status == Status.Valid);
                undercoverLexicons = undercoverLexicons.Where(w => w.Uid != eventArgs.Sender.Id).ToList();
                //创建房间
                var lexicon = undercoverLexicons[new Random().Next(0, undercoverLexicons.Count)];
                var roomId = _undercoverServices.InsertR(new UndercoverRoom()
                {
                    CreateUid = eventArgs.Sender.Id,
                    GroupId = eventArgs.SourceGroup.Id,
                    IsStart = false,
                    UndercoverLexiconId = lexicon.ID
                });
                //
                await SendMessageGroup(eventArgs, $"已成功创建【{roomId}】号房间");
            }
        }

        /// <summary>
        /// <seealso cref="IUndercoverInterface.RoomList(GroupMessageEventArgs)"/>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask RoomList(GroupMessageEventArgs eventArgs)
        {
            var rooms = _undercoverServices.Query(w => w.GroupId == eventArgs.SourceGroup.Id && w.IsStart == false);
            if (rooms.Count <= 0)
                await SendMessageGroup(eventArgs, "暂时没有等待开始的房间", false, true);
            var strBuilder = new StringBuilder();
            strBuilder.Append($"房间列表：");
            foreach (var room in rooms)
            {
                strBuilder.Append($"\r\n【{room.ID}】号房");
            }

            await SendMessageGroup(eventArgs, strBuilder.ToString());
        }

        /// <summary>
        /// <seealso cref="IUndercoverInterface.DissolveRoom(GroupMessageEventArgs)"/>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask DissolveRoom(GroupMessageEventArgs eventArgs)
        {
            //jsfj1
            _ = int.TryParse(eventArgs.Message.RawText.Split("jsfj")[1], out var roomId);
            var room = _undercoverServices.Query(w => w.ID == roomId).FirstOrDefault();
            if (room is null)
                await SendMessageGroup(eventArgs, "未找到房间信息", true, true);
            if(room.CreateUid!=eventArgs.SenderInfo.UserId || room.IsStart)
                await SendMessageGroup(eventArgs, "您无权解散该房间", true, true);
            room.IsStart = false;
            room.Status = Status.InValid;
            _undercoverServices.Update(room);
            await SendMessageGroup(eventArgs, "房间已解散", true);
        }

        /// <summary>
        /// <seealso cref="IUndercoverInterface.Fqtp(GroupMessageEventArgs)"/>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask Fqtp(GroupMessageEventArgs eventArgs)
        {
            await SendMessageGroup(eventArgs, "请开始投票，30秒之后公布结果");
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000 * 30);
                var room = _undercoverServices.Query(w => w.CreateUid == eventArgs.Sender.Id && w.IsStart).FirstOrDefault();
                var result = GlobalSettings.TpResult.Where(w => w.RoomId == room.ID).ToList();
                if (result.Count <= 0)
                    await SendMessageGroup(eventArgs, "本轮平票，请开始下轮发言", false, true);
                //票数最高
                var uid = result.GroupBy(g => g.Uid).OrderByDescending(o => o.Count()).FirstOrDefault().Key;
                var users = _undercoverUserServices.Query(w => w.RoomId == room.ID);
                var updateUser = users.FirstOrDefault(f => f.Uid == uid);
                if (users.Count(w => w.IsOut == false) > 3 && updateUser.IsUndercover is false)
                {
                    updateUser.IsOut = true;
                    _undercoverUserServices.Update(updateUser);
                    //禁言票出人员
                    await eventArgs.SoraApi.EnableGroupMemberMute(eventArgs.SourceGroup.Id, uid, 1000 * 60 * 10);
                    await SendMessageGroup(eventArgs, $"本轮【{updateUser.Nick}】被票出，请开始下轮发言");
                    var str = new StringBuilder("本轮票型：");
                    foreach (var item in result.GroupBy(g => g.Uid).OrderByDescending(o => o.Count()))
                    {
                        var user = users.FirstOrDefault(f => f.Uid == item.Key);
                        str.Append($"【{user.Nick}】     {item.Count()}票");
                    }
                    await SendMessageGroup(eventArgs, str.ToString());
                }
                else
                {
                    bool isWin = (users.Count(w => w.IsOut == false) <= 3 && users.Any(w => w.IsUndercover)) || updateUser.IsUndercover is false;
                    await StopGame(eventArgs, isWin);
                }
            });
        }

        /// <summary>
        /// <seealso cref="IUndercoverInterface.Vote(GroupMessageEventArgs)"/>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask Vote(GroupMessageEventArgs eventArgs)
        {
            //tpy1
            _ = int.TryParse(eventArgs.Message.RawText.Split("tpy")[1], out var uIndx);
            var users = _undercoverUserServices.Query(w => w.Uid == eventArgs.Sender.Id);
            var roomIds = users.Select(s => s.RoomId).ToList();
            var room = _undercoverServices.Query(w => roomIds.Contains(w.ID) && w.IsStart && w.Status == Status.Valid).FirstOrDefault();
            if (room is null)
                await SendMessageGroup(eventArgs, $"投票失败，未找到有效房间信息", true, true);
            if(GlobalSettings.TpResult.Any(a=>a.RoomId== room.ID && a.TpUid == eventArgs.Sender.Id))
                await SendMessageGroup(eventArgs, $"已经投过票了", true, true);
            GlobalSettings.TpResult.Add(new GlobalSettings.TpResults()
            {
                RoomId = room.ID,
                Num = 1,
                TpUid = eventArgs.Sender.Id,
                Uid = users[uIndx - 1].Uid
            });
        }

        #region Private Method

        /// <summary>
        /// 初始化词库
        /// </summary>
        private async ValueTask InitalizeLexicon()
        {
            List<(string, string)> lexicon = new()
            {
                ("麻雀","乌鸦"),
                ("玫瑰","月季"),
                ("海豹","海狮"),
                ("鲸鱼","鲨鱼"),
                ("老虎","狮子"),
                ("蝴蝶","蜜蜂"),
                ("鹦鹉","鸽子"),
                ("哈士奇","吉娃娃"),
                ("大熊猫","小熊猫"),
                ("梁山伯与祝英台","罗密欧与朱丽叶"),
                ("气泡","水泡"),
                ("唇膏","口红"),
                ("烤肉","涮肉"),
                ("葡萄","提子"),
                ("橙子","橘子"),
                ("杭州","苏州"),
                ("状元","冠军"),
                ("保安","保镖"),
                ("双胞胎","龙凤胎"),
                ("班主任","辅导员"),
                ("男朋友","前男友"),
                ("富二代","高富帅"),
                ("老婆","媳妇"),
                ("眉毛","胡须"),
                ("果粒橙","鲜橙多"),
                ("雪糕","冰棍"),
                ("泡泡糖","口香糖"),
                ("牛肉干","猪肉脯"),
                ("面包","蛋糕"),
                ("自行车","电动车"),
                ("过山车","碰碰车"),
                ("九阴白骨爪","降龙十八掌"),
                ("奥特曼","小怪兽"),

            };
            lexicon.ForEach(f =>
            {
                _undercoverLexiconServices.Insert(new UndercoverLexicon()
                {
                    UndercoverWord = f.Item1,
                    Word = f.Item2
                });
            });

            await Task.Delay(1);
        }

        /// <summary>
        /// 发送群组临时会话
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="uid"></param>
        /// <param name="groupId"></param>
        /// <param name="strContent"></param>
        /// <returns></returns>
        private async ValueTask SendTemporaryMessage(GroupMessageEventArgs eventArgs, long uid,long groupId,string strContent)=> await eventArgs.SourceGroup.SoraApi.SendTemporaryMessage(uid, groupId, strContent);

        /// <summary>
        /// 发送群组消息
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="strContent"></param>
        /// <param name="isAt">是否艾特</param>
        /// <returns></returns>
        private static async ValueTask SendMessageGroup(GroupMessageEventArgs eventArgs, string strContent, bool isAt = false,bool isBreak = false)
        {
            if (isAt)
            {
                var msg = new MessageBody()
                {
                    SoraSegment.At(eventArgs.Sender.Id),
                    SoraSegment.Text(strContent)
                };
                await eventArgs.Reply(msg);
            }
            else
                await eventArgs.Reply(strContent);
            if (isBreak)
                throw new InterruptException();
        }

        /// <summary>
        /// 发送群组消息
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="messageBody"></param>
        /// <returns></returns>
        private async ValueTask SendMessageGroup(GroupMessageEventArgs eventArgs, MessageBody messageBody) => await eventArgs.SourceGroup.SendGroupMessage(messageBody);
        #endregion
    }
}
