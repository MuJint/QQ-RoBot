using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Robot.Common.Helper;
using Robot.Common.Interface;
using Robot.Framework.Interface;
using Robot.Framework.Services;
using System;
using System.Threading.Tasks;

namespace QQ.RoBot
{
    static class BotStartUp
    {
        static async Task Main()
        {
            try
            {
                Host.CreateDefaultBuilder()
                    .ConfigureHostConfiguration(builder =>
                    {
                        builder.AddJsonFile("appsettings.json");
                    })
                    .ConfigureServices((builder, services) =>
                    {
                        var config = builder.Configuration;
                        config.Bind(GlobalSettings.AppSetting);
                        var s = GlobalSettings.AppSetting;

                        //日志操作
                        services.AddScoped<ILogsInterface, LogsHelper>();

                        //数据库操作
                        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

                        //功能实现
                        services.AddScoped<IRobotInterface, RobotService>();
                        services.AddScoped<ILianInterface, LianService>();
                        services.AddScoped<IHsoInterface, HsoService>();
                        services.AddScoped<IUndercoverInterface, UndercoverService>();
                        services.AddHostedService<IWorker>();

                    })
                    .ConfigureAppConfiguration((builder, app) =>
                    {
                    })
                    .ConfigureLogging((builder, logging) =>
                    {
                    })
                .UseConsoleLifetime()
                .Build()
                .Run();
            }
            catch (Exception ex)
            {
                new LogsHelper().Error(ex, "程序崩溃");
            }
            //Bubble sort algorithm
            await Task.Delay(1);
        }
    }
}
