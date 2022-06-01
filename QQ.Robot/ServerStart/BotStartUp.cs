using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Robot.Common.Helper;
using Robot.Common.Interface;
using Robot.Framework.Interface;
using Robot.Framework.Services;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    static class BotStartUp
    {
        static async Task Main()
        {
            Host.CreateDefaultBuilder().ConfigureServices((builder, services) =>
            {
                var config = builder.Configuration;
                config.Bind(GlobalSettings.AppSetting);
                var s = GlobalSettings.AppSetting;
                services.AddScoped<ILogsInterface, LogsHelper>();
                services.AddScoped<ILianChatServices, LianChatServices>();
                services.AddScoped<ILianKeyWordsServices, LianKeyWordsServices>();
                services.AddScoped<ISignLogsServices, SignLogsServices>();
                services.AddScoped<ISignUserServices, SignUserServices>();
                services.AddScoped<ISpeakerServices, SpeakerServices>();
                services.AddScoped<IRobotInterface, RobotService>();
                services.AddScoped<ILianInterface, LianService>();
                services.AddScoped<IHsoInterface, HsoService>();
                services.AddHostedService<IWorker>();
            })
            .ConfigureHostConfiguration(builder => 
            {
                builder.AddJsonFile("appsettings.json");
            })
            .Build()
            .Run();

            await Task.Delay(1);
        }
    }
}
