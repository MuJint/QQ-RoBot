using Microsoft.Extensions.DependencyInjection;

namespace QQ.RoBot
{
    public abstract class BaseServiceObject
    {
        protected static T GetInstance<T>() => Dependcy.Provider.GetService<T>();
    }
}
