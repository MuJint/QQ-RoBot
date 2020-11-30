using Qiushui.Lian.Bot.Framework.IRepository;
using Qiushui.Lian.Bot.Framework.IServices;
using Qiushui.Lian.Bot.Models;
using System.Threading.Tasks;

namespace Qiushui.Lian.Bot.Framework.Services
{
    class SignLogsServices : BaseServices<SignLogs>, ISignLogsServices
    {
        readonly ISignUserRepository _dal;
      
    }
}
