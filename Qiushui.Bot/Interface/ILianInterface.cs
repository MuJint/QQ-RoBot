using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace Qiushui.Bot
{
    /// <summary>
    /// LianInterface
    /// <para>天刀整容团莲的主要接口</para>
    /// </summary>
    public interface ILianInterface
    {
        ValueTask SignIn(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask SearchRank(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask Fenlai(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask Skill(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask RankList(GroupMessageEventArgs eventArgs);
        ValueTask SpecialEvent(GroupMessageEventArgs eventArgs);
        ValueTask LogsRecord(GroupMessageEventArgs eventArgs);
        ValueTask Giving(GroupMessageEventArgs eventArgs);
        ValueTask Morning(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask Night(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask BonusPoint(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask DeductPoint(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask AllBonusPoint(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask AllDeductPoint(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask RedTea(GroupMessageEventArgs eventArgs);
        ValueTask Raffle(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask Rob(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask Rescur(GroupMessageEventArgs eventArgs, UserConfig config);
        ValueTask Lian(GroupMessageEventArgs eventArgs);
        ValueTask AddKeys(GroupMessageEventArgs eventArgs);
        ValueTask AddThesaurus(GroupMessageEventArgs eventArgs);
        ValueTask RollDice(GroupMessageEventArgs eventArgs);
        ValueTask WordCloud(GroupMessageEventArgs eventArgs);
        ValueTask NonsenseKing(GroupMessageEventArgs eventArgs);
    }
}
