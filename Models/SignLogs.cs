using System.ComponentModel;

namespace Qiushui.Lian.Bot.Models
{
    public class SignLogs : BaseModel
    {
        public string Uid { get; set; }
        public string LogContent { get; set; }
        public int ModifyRank { get; set; }
        public CmdType CmdType { get; set; } = CmdType.SignIn;
    }

    public enum CmdType
    {
        /// <summary>
        /// 签到
        /// </summary>
        [Description("签到")]
        SignIn = 1,
        /// <summary>
        /// 加分
        /// </summary>
        [Description("加分")]
        BonusPoints = 2,
        /// <summary>
        /// 扣分
        /// </summary>
        [Description("扣分")]
        PointsDeducted = 3,
        /// <summary>
        /// 特殊扣分
        /// </summary>
        [Description("特殊扣分")]
        SpecialPointsDeducted = 4,
        /// <summary>
        /// 特殊加分
        /// </summary>
        [Description("特殊加分")]
        SpecialBonusPoints = 5,
    }
}
