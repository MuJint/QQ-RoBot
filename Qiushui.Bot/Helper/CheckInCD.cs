using System;
using System.Collections.Generic;

namespace Qiushui.Bot
{
    public class CheckInCD
    {
        #region 调用记录用结构体
        private struct CheckUser
        {
            public long GroupId { get; set; }
            public long UserId  { get; set; }
        }
        #endregion

        #region 调用时间记录Dictionary
        /// <param type="long">QQ号</param>
        /// <param type="DateTime">上次调用时间</param>
        private static readonly Dictionary<CheckUser, DateTime> LastChatDate = new();
        #endregion

        #region 调用时间检查
        /// <summary>
        /// 检查用户调用时是否在CD中
        /// 对任何可能刷屏的指令都有效
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否在CD中</returns>
        public static bool IsInCD(long groupId, long userId)
        {
            DateTime time = DateTime.Now; //获取当前时间
            CheckUser user = new()
            {
                GroupId = groupId,
                UserId  = userId
            };
            //尝试从字典中取出上一次调用的时间
            if (LastChatDate.TryGetValue(user, out DateTime last_use_time) &&
                (long)(time - last_use_time).TotalSeconds < 60)
            {
                //刷新调用时间
                LastChatDate[user] = time;
                return true;
            }
            else
            {
                //刷新/写入调用时间
                LastChatDate[user] = time;
                return false;
            }
        }
        #endregion
    }
}
