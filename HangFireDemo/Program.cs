using Hangfire;
using HangFire.JobInterface;
{
    GlobalConfiguration.Configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)//兼容性等级
    .UseColouredConsoleLogProvider()//日志组件
    .UseSimpleAssemblyNameTypeSerializer()//序列化器组件
    .UseRecommendedSerializerSettings()//全局序列化器
    //存储介质SqlServer
    .UseSqlServerStorage(@" TrustServerCertificate=true;server=.;database=Hangfire;uid=sa;pwd=123")
    ;
}

#region 普通任务
/*BackgroundJob.Enqueue(() => Console.WriteLine($"欢迎兄弟们来和我一起研究HangFireJob，我只运行一次，当前时间：{DateTime.Now}"));

RecurringJob.AddOrUpdate(() => Console.WriteLine(
    $"欢迎兄弟们来和我一起研究HangFireJob，当前时间：{DateTime.Now}"),
    Cron.Minutely());
var jobId1 = BackgroundJob.Schedule(() => Console.WriteLine($"这里是延迟10秒后执行的操作，当前时间：{DateTime.Now}"), TimeSpan.FromSeconds(10));
*/
#endregion

#region 接口调度任务
//多行任务，转移封装
BackgroundJob.Enqueue<SynchDataService>(work => work.RefreshSysUserOnCompanyName());

RecurringJob.AddOrUpdate<SynchDataService>(work => work.RefreshSysUserOnCompanyName(), "* * * * * ?");

#endregion


using (BackgroundJobServer server = new BackgroundJobServer())
{
    Console.ReadLine();
}
