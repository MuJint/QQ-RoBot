using System.ComponentModel;

namespace Qiushui.Lian.Bot.Models
{
    /// <summary>
    /// 机器人指令
    /// </summary>
    internal enum PCRGuildBattleCommand
    {
        /// <summary>
        /// 建会
        /// </summary>
        [Description("建会")]
        CreateGuild = 1,
        /// <summary>
        /// 入会
        /// </summary>
        [Description("入会")]
        JoinGuild = 2,
        /// <summary>
        /// 查看成员
        /// </summary>
        [Description("查看成员")]
        ListMember = 3,
        /// <summary>
        /// 退会
        /// </summary>
        [Description("退会")]
        QuitGuild = 4,
        /// <summary>
        /// 清空成员
        /// </summary>
        [Description("一键退会")]
        QuitAll = 5,
        /// <summary>
        /// 一键入会
        /// </summary>
        [Description("一键入会")]
        JoinAll = 6,
        /// <summary>
        /// 删除公会
        /// </summary>
        [Description("删除公会")]
        DeleteGuild = 7,
        /// <summary>
        /// 会战开始指令
        /// </summary>
        [Description("会战开始")]
        BattleStart = 101,
        /// <summary>
        /// 会战结束命令
        /// </summary>
        [Description("会战结束")]
        BattleEnd = 102,
        /// <summary>
        /// 申请出刀命令
        /// </summary>
        [Description("申请出刀")]
        RequestAttack = 103,
        /// <summary>
        /// 出刀命令
        /// </summary>
        [Description("出刀")]
        Attack = 104,
        /// <summary>
        /// 删刀命令
        /// </summary>
        [Description("删刀")]
        DeleteAttack = 105,
        /// <summary>
        /// 撤销刀命令
        /// </summary>
        [Description("撤刀")]
        UndoAttack = 106,
        /// <summary>
        /// SL命令
        /// </summary>
        [Description("SL")]
        SL = 107,
        /// <summary>
        /// 撤回SL命令
        /// </summary>
        [Description("撤回SL")]
        UndoSL = 108,
        /// <summary>
        /// 查看进度命令
        /// </summary>
        [Description("进度")]
        ShowProgress = 109,
        /// <summary>
        /// 挂树命令
        /// </summary>
        [Description("挂树")]
        ClimbTree = 110,
        /// <summary>
        /// 查树命令
        /// </summary>
        [Description("查树")]
        ShowTree = 111,
        /// <summary>
        /// 下树命令
        /// </summary>
        [Description("下树")]
        LeaveTree = 112,
        /// <summary>
        /// 全出刀表命令
        /// </summary>
        [Description("公会出刀表")]
        ShowAllAttackList = 113,
        /// <summary>
        /// 查余刀命令
        /// </summary>
        [Description("查刀")]
        ShowRemainAttack = 114,
        /// <summary>
        /// 催刀命令
        /// </summary>
        [Description("催刀")]
        UrgeAttack = 115,
        /// <summary>
        /// 取消出刀申请
        /// </summary>
        [Description("取消出刀")]
        UndoRequestAtk = 116,
        /// <summary>
        /// 修改进度
        /// </summary>
        [Description("修改进度")]
        ModifyProgress = 117,
        /// <summary>
        /// 单人出刀表查询
        /// </summary>
        [Description("出刀查询")]
        ShowAttackList = 118,
    }
}
