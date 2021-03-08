using Sora.EventArgs.SoraEvent;
using Sora.Tool;
using System.Threading.Tasks;

namespace Qiushui.Bot.ServerInterface
{
    /// <summary>
    /// 群成员变更事件
    /// </summary>
    internal class GroupMemberChangeEvent
    {
        public static async ValueTask GroupMemberChangeParse(object sender, GroupMemberChangeEventArgs eventArgs)
        {
            var userInfo = await eventArgs.ChangedUser.GetUserInfo();
            try
            {
                var @operator = await eventArgs.Operator.GetUserInfo();
                switch (eventArgs.SubType)
                {
                    case Sora.Enumeration.EventParamsType.MemberChangeType.Leave:
                        await eventArgs.SourceGroup.SendGroupMessage($"[{userInfo.userInfo.Nick}]离开了..原因未知");
                        break;
                    case Sora.Enumeration.EventParamsType.MemberChangeType.Kick:
                        await eventArgs.SourceGroup.SendGroupMessage($"[{userInfo.userInfo.Nick}]被[{@operator.userInfo.Nick}]强行请离了");
                        break;
                    case Sora.Enumeration.EventParamsType.MemberChangeType.KickMe:
                        break;
                    case Sora.Enumeration.EventParamsType.MemberChangeType.Approve:
                        await eventArgs.SourceGroup.SendGroupMessage($"[{@operator.userInfo.Nick}]同意[{userInfo.userInfo.Nick}]加入了群聊");
                        break;
                    case Sora.Enumeration.EventParamsType.MemberChangeType.Invite:
                        await eventArgs.SourceGroup.SendGroupMessage($"[{@operator.userInfo.Nick}]邀请[{userInfo.userInfo.Nick}]加入了群聊");
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception c)
            {
                //此处存在一个异常
                //System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. (Parameter 'userId')
                ConsoleLog.Error("Error", c.Message);
                ConsoleLog.Error("Error", $"操作失败者：{eventArgs.Operator.Id}");
                await eventArgs.SourceGroup.SendGroupMessage($"[{userInfo.userInfo.Nick}]未知状态..请查看相关日志");
            }
        }
    }
}
