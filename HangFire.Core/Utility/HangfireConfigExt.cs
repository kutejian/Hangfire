using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.HttpJob;
using Hangfire.SqlServer;
using Hangfire.Tags.SqlServer;
using Microsoft.Extensions.Options;

namespace Steven.HangFire.Core.Utility
{
    public static class HangfireConfigExt
    {

        public static void CustomAddHangfire(this WebApplicationBuilder builder)
        {
            #region 配置Hangfire
          //添加 Hangfire配置
          builder.Services.AddHangfire(configuration => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions() //Nuget引入：
               {
                   CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                   SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                   QueuePollInterval = TimeSpan.Zero,
                   UseRecommendedIsolationLevel = true,
                   UsePageLocksOnDequeue = true,
                   DisableGlobalLocks = true

               }).UseTagsWithSql()//nuget引入Hangfire.Tags.SqlServer
                 .UseConsole(new ConsoleOptions()
                 {
                     BackgroundColor = "#000079"

                 }));
            builder.Services.AddHangfireServer(); //Hangfire的服务也添加到容器中去

            #endregion
            #region HttpJob
/*            // 支持HttpJob:
            //1.nuget引入:Hangfire.HttpJob
            //2. .UseHangfireHttpJob(
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions()
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                })
                .UseTagsWithSql()
                .UseConsole(new ConsoleOptions { BackgroundColor = "#000079" })
                //支持httpJob
                .UseHangfireHttpJob()
                );        
            builder.Services.AddHangfireServer(); //Hangfire的服务也添加到容器中去*/
            #endregion
        }


        public static void UseCustomHangfire(this WebApplication app)
        {
            #region 默认的申请认证 配置使用面板
            //可视化面板的授权--集成ASP.NET Core授权
/*            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                //启用所有的权限
                IsReadOnlyFunc = (DashboardContext context) => true,
                //配置权限
                Authorization = new CustomDashboardAuthorizationFilter[] { new CustomDashboardAuthorizationFilter() },
                DashboardTitle = "Steven定时作业-Hangfire"
            });*/
            #endregion


            #region hangfire自定义授权
            //读取授权信息
            User user = app.Services.GetRequiredService<IOptionsMonitor<User>>().CurrentValue;
            BasicAuthAuthorizationUser[] userList = user.BasicAuthAuthorizationUser
                .Select(c => new BasicAuthAuthorizationUser()
                {
                    Login = c.Name,
                    PasswordClear = c.Password
                }).ToArray();
            //支持可视化界面 ---任何一个用户都能够来访问；所以需要加一道屏障
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                //这里是默认实现，如果使用这个，小心坑：nuget引入：Hangfire.Dashboard.BasicAuthorization
                Authorization = new BasicAuthAuthorizationFilter[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        SslRedirect=false,
                        RequireSsl=false,
                        Users=userList
                    })
                }
            });
            #endregion
        }

    }
}
