using Qiushui.Bot.Framework.IRepository;
using Qiushui.Bot.Framework.IServices;
using Qiushui.Bot.Models;

namespace Qiushui.Bot.Framework.Services
{
    public class SignLogsServices : BaseServices<SignLogs>, ISignLogsServices
    {
        readonly ISignUserRepository _dal;
      
    }
}
