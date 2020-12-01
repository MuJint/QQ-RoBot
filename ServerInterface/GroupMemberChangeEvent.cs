using Sora.EventArgs.SoraEvent;
using System.Threading.Tasks;

namespace Qiushui.Lian.Bot.ServerInterface
{
    /// <summary>
    /// 群成员变更事件
    /// </summary>
    internal class GroupMemberChangeEvent
    {
        public static async ValueTask GroupMemberChangeParse(object sender, GroupMemberChangeEventArgs eventArgs)
        {
            var userInfo = await eventArgs.ChangedUser.GetUserInfo();
            var @operator = await eventArgs.Operator.GetUserInfo();
            switch (eventArgs.SubType)
            {
                case Sora.Enumeration.EventParamsType.MemberChangeType.Leave:
                    await eventArgs.SourceGroup.SendGroupMessage($"{userInfo.userInfo.Nick}离开了");
                    break;
                case Sora.Enumeration.EventParamsType.MemberChangeType.Kick:
                    await eventArgs.SourceGroup.SendGroupMessage($"【{userInfo.userInfo.Nick}】被【{@operator.userInfo.Nick}】强行请离了");
                    break;
                case Sora.Enumeration.EventParamsType.MemberChangeType.KickMe:
                    break;
                case Sora.Enumeration.EventParamsType.MemberChangeType.Approve:
                    await eventArgs.SourceGroup.SendGroupMessage($"{@operator.userInfo.Nick}同意{userInfo.userInfo.Nick}加入了群聊");
                    break;
                case Sora.Enumeration.EventParamsType.MemberChangeType.Invite:
                    await eventArgs.SourceGroup.SendGroupMessage($"{@operator.userInfo.Nick}邀请{userInfo.userInfo.Nick}加入了群聊");
                    break;
                default:
                    break;
            }
        }
    }
}
