using Qiushui.Lian.Bot.Framework.IRepository;
using Qiushui.Lian.Bot.Framework.IServices;
using Qiushui.Lian.Bot.Models;

namespace Qiushui.Lian.Bot.Framework.Services
{
    public class SignLogsServices : BaseServices<SignLogs>, ISignLogsServices
    {
        readonly ISignUserRepository _dal;
      
    }
}
