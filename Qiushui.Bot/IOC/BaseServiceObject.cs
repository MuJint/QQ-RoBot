using Microsoft.Extensions.DependencyInjection;

namespace Qiushui.Bot
{
    public abstract class BaseServiceObject
    {
        protected static T GetInstance<T>() => Dependcy.Provider.GetService<T>();
    }
}
