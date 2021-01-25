using Qiushui.Bot.Models;
using Sora.Entities;
using Sora.Entities.CQCodes;
using Sora.Entities.Info;
using System;
using System.Text;

namespace Qiushui.Bot.Helper
{
    internal static class BotUtils
    {
        #region 时间戳处理
        /// <summary>
        /// 获取今天零点的时间戳
        /// 时间戳单位(秒)
        /// </summary>
        public static long GetTodayStampLong() =>(long) (DateTime.Today - new DateTime(1970, 1, 1, 8, 0, 0, 0)).TotalSeconds;

        /// <summary>
        /// 获取游戏刷新的时间戳
        /// 时间戳单位(秒)
        /// </summary>
        public static long GetUpdateStamp()
        {
            if (DateTime.Now > DateTime.Today.Add(new TimeSpan(5, 0, 0)))
            {
                return (long)( DateTime.Today - new DateTime(1970, 1, 1, 8, 0, 0, 0)).Add(new TimeSpan(5, 0, 0)).TotalSeconds;
            }
            else
            {
                return (long)( DateTime.Today.AddDays(-1) - new DateTime(1970, 1, 1, 8, 0, 0, 0)).Add(new TimeSpan(5, 0, 0)).TotalSeconds;
            } 
        }
        #endregion

        #region 群成员处理
        /// <summary>
        /// 获取群成员的名片，没有则获取昵称
        /// </summary>
        public static string getNick(GroupMemberInfo input)
        {
            return input.Card == "" ? input.Nick : input.Card;
        }
        #endregion

        #region 字符串处理
        /// <summary>
        /// 获取字符串在QQ上显示的长度（用于PadQQ函数）
        /// </summary>
        /// <param name="input">要计算长度的字符串</param>
        /// <returns>长度（不要问为啥是Double，0.5个字符真的存在）</returns>
        public static double GetQQStrLength(string input)
        {
            double strLength = 0;
            foreach (char i in input)
            {
                if (Char.IsLetter(i))
                {
                    strLength += 2.5;
                }
                else if (Char.IsNumber(i))
                {
                    strLength += 2;
                }
                else if (Char.IsSymbol(i))
                {
                    strLength += 2;
                }
                else
                {
                    strLength += 3;
                }
            }

            return strLength;
        }

        /// <summary>
        /// 对字符串进行PadRight，但符合QQ上的对齐标准
        /// </summary>
        /// <param name="input">要补齐的字符串</param>
        /// <param name="padNums">补齐的长度（请使用getQQStrLength进行计算）</param>
        /// <param name="paddingChar">用来对齐的字符（强烈建议用默认的空格，其他字符请手动计算后用String类原生的PadRight进行操作）</param>
        /// <returns>补齐长度后的字符串</returns>
        public static string PadRightQQ(string input, double padNums, char paddingChar = ' ')
        {
            StringBuilder sb = new StringBuilder();

            int toPadNum = (int) Math.Floor(padNums - GetQQStrLength(input));
            if (toPadNum <= 0)
            {
                return input;
            }
            else
            {
                sb.Append(input);
                for (int i = 0; i < toPadNum; i++)
                {
                    sb.Append(paddingChar);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 检查参数数组长度
        /// </summary>
        /// <param name="args">指令数组</param>
        /// <param name="len">至少需要的参数个数</param>
        /// <param name="QQgroup">（可选，不给的话就不发送错误信息）\n报错信息要发送到的QQ群对象</param>
        /// <param name="fromQQid">（可选，但QQgroup给了的话本参数必填）\n要通知的人的QQ Id</param>
        /// <returns>Illegal不符合 Legitimate符合 Extra超出</returns>
        public static LenType CheckForLength(string[] args, int len, Group QQgroup = null, long fromQQid = 0)
        {
            if (args.Length >= len + 1)
            {
                if (args.Length == len + 1) return LenType.Legitimate;
                else return LenType.Extra;
            }
            else
            {
                QQgroup?.SendGroupMessage(CQCode.CQAt(fromQQid), " 命令参数不全，请补充。");
                return LenType.Illegal;
            }
        }
        #endregion
    }
}
