using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Robot.Framework.Models
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
        [Display(Name = "SignIn", Description = "签到")]
        SignIn = 1,
        /// <summary>
        /// 加分
        /// </summary>
        [Display(Name = "BonusPoints", Description = "加分")]
        BonusPoints = 2,
        /// <summary>
        /// 扣分
        /// </summary>
        [Display(Name = "PointsDeducted", Description = "扣分")]
        PointsDeducted = 3,
        /// <summary>
        /// 特殊扣分
        /// </summary>
        [Display(Name = "SpecialPointsDeducted", Description = "特殊扣分")]
        SpecialPointsDeducted = 4,
        /// <summary>
        /// 特殊加分
        /// </summary>
        [Display(Name = "SpecialBonusPoints", Description = "特殊加分")]
        SpecialBonusPoints = 5,
        /// <summary>
        /// 通过赠送获取积分
        /// </summary>
        [Display(Name = "Giving", Description = "通过赠送获取积分")]
        Giving = 6,
        /// <summary>
        /// 通过抢劫获取积分
        /// </summary>
        [Display(Name = "Rob", Description = "通过抢劫获取积分")]
        Rob =7,
        /// <summary>
        /// 通过抢劫反杀获取积分
        /// </summary>
        [Display(Name = "DeRob", Description = "通过抢劫反杀获取积分")]
        DeRob =8,
    }
}
