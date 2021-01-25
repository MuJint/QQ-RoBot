using Qiushui.Bot.Framework.IRepository;
using Qiushui.Bot.Framework.IServices;
using Qiushui.Bot.Models;
using System.Threading.Tasks;

namespace Qiushui.Bot.Framework.Services
{
    public class SignUserServices : BaseServices<SignUser>, ISignUserServices
    {
        readonly ISignUserRepository _dal;
      
    }
}
