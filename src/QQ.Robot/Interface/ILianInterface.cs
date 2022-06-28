using Robot.Common;
using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    /// <summary>
    /// LianInterface
    /// <para>天刀整容团莲的主要接口</para>
    /// </summary>
    public interface ILianInterface
    {
        [KeyWord("签到", true)]
        ValueTask SignIn(GroupMessageEventArgs eventArgs);
        [KeyWord("查询", true)]
        ValueTask SearchRank(GroupMessageEventArgs eventArgs);
        [KeyWord("分来", true)]
        ValueTask Fenlai(GroupMessageEventArgs eventArgs);
        [KeyWord("技能 菜单 功能 指令", true)]
        ValueTask Skill(GroupMessageEventArgs eventArgs);
        [KeyWord("排行榜", true)]
        ValueTask RankList(GroupMessageEventArgs eventArgs);
        [KeyWord("特殊事件", true)]
        ValueTask SpecialEvent(GroupMessageEventArgs eventArgs);
        [KeyWord("积分记录 个人积分")]
        ValueTask LogsRecord(GroupMessageEventArgs eventArgs);
        [KeyWord("赠送")]
        ValueTask Giving(GroupMessageEventArgs eventArgs);
        [KeyWord("早安", true)]
        ValueTask Morning(GroupMessageEventArgs eventArgs);
        [KeyWord("晚安", true)]
        ValueTask Night(GroupMessageEventArgs eventArgs);
        [KeyWord("加分")]
        ValueTask BonusPoint(GroupMessageEventArgs eventArgs);
        [KeyWord("扣分")]
        ValueTask DeductPoint(GroupMessageEventArgs eventArgs);
        [KeyWord("全体加分")]
        ValueTask AllBonusPoint(GroupMessageEventArgs eventArgs);
        [KeyWord("全体扣分")]
        ValueTask AllDeductPoint(GroupMessageEventArgs eventArgs);
        [KeyWord("优质睡眠 昏睡红茶 昏睡套餐 健康睡眠")]
        ValueTask RedTea(GroupMessageEventArgs eventArgs);
        [KeyWord("抽奖", true)]
        ValueTask Raffle(GroupMessageEventArgs eventArgs);
        [KeyWord("打劫")]
        ValueTask Rob(GroupMessageEventArgs eventArgs);
        [KeyWord("救援 劫狱")]
        ValueTask Rescur(GroupMessageEventArgs eventArgs);
        [KeyWord("莲", true)]
        ValueTask Lian(GroupMessageEventArgs eventArgs);
        [KeyWord("添加数据密码")]
        ValueTask AddKeys(GroupMessageEventArgs eventArgs);
        [KeyWord("添加词库")]
        ValueTask AddThesaurus(GroupMessageEventArgs eventArgs);
        [KeyWord("骰子 扔骰子 掷骰子 色子")]
        ValueTask RollDice(GroupMessageEventArgs eventArgs);
        [KeyWord("词云", true)]
        ValueTask WordCloud(GroupMessageEventArgs eventArgs);
        [KeyWord("发言榜", true)]
        ValueTask NonsenseKing(GroupMessageEventArgs eventArgs);
    }
}
