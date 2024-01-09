## [HangFire](https://www.hangfire.io/)

### 定时作业的认识：

同步任务：如数据库冗余同步，定时刷盘，定时轮询某些事情等

.Net中常见的定时作业框架（主流）

- QuartZ
- HangFire

![image-20230613205247617](D:\朝夕\资料\讲义知识整理\TyporaImages\image-20230613205247617.png)

---

### HangFire上手：

老规矩，第一步，导包：

```C#
<ItemGroup>
    <PackageReference Include="Hangfire" Version="1.8.2" />
    <PackageReference Include="Hangfire.Core" Version="1.8.2" />
    <PackageReference Include="Hangfire.SqlServer" Version="1.8.2" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.1" />
  </ItemGroup>
```

---

第二步，配置存储介质（持久化）：

```C#
// 需要自己先创建好 HangFireDemo 这个数据库，它没那么智能
GlobalConfiguration.Configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)//兼容性等级
    .UseColouredConsoleLogProvider()//日志组件
    .UseSimpleAssemblyNameTypeSerializer()//序列化器组件
    .UseRecommendedSerializerSettings()//全局序列化器
    //存储介质SqlServer
    .UseSqlServerStorage(@"Data Source=LAPTOP-AVFER53P\SQLEXPRESS;Initial Catalog=HangFireDemo;User ID=sa;Password=123456;TrustServerCertificate=true")
    ;
```

---

第三步，添加任务到DB：

```C#
using Hangfire;
{
    GlobalConfiguration.Configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)//兼容性等级
    .UseColouredConsoleLogProvider()//日志组件
    .UseSimpleAssemblyNameTypeSerializer()//序列化器组件
    .UseRecommendedSerializerSettings()//全局序列化器
    //存储介质SqlServer
    .UseSqlServerStorage(@"Data Source=LAPTOP-AVFER53P\SQLEXPRESS;Initial Catalog=HangFireDemo;User ID=sa;Password=123456;TrustServerCertificate=true")
    ;
}
BackgroundJob.Enqueue(() => Console.WriteLine($"欢迎兄弟们来和我一起研究HangFireJob，我只运行一次，当前时间：{DateTime.Now}"));

RecurringJob.AddOrUpdate(() => Console.WriteLine(
    $"欢迎兄弟们来和我一起研究HangFireJob，当前时间：{DateTime.Now}"),
    Cron.Minutely());
var jobId1 = BackgroundJob.Schedule(() => Console.WriteLine($"这里是延迟10秒后执行的操作，当前时间：{DateTime.Now}"), TimeSpan.FromSeconds(10));
```

然后运行启动即可

```C#
using (BackgroundJobServer server = new BackgroundJobServer())
{
    Console.ReadLine();
}
```

---

### WebApi中的操作

#### 老规矩，第一步，导包：

```C#
 <ItemGroup>
    <PackageReference Include="Hangfire" Version="1.8.2" />
    <PackageReference Include="Hangfire.AspNetCore" Version="1.8.2" />
    <PackageReference Include="Hangfire.Core" Version="1.8.2" />
    <PackageReference Include="Hangfire.Dashboard.BasicAuthorization" Version="1.0.2" />
    <PackageReference Include="Hangfire.HttpJob" Version="3.7.9" />
    <PackageReference Include="Hangfire.SqlServer" Version="1.8.2" />
    <PackageReference Include="Hangfire.Tags.SqlServer" Version="1.8.2" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.1" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="6.0.14" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.2.3" />
  </ItemGroup>
```

#### 第二步，配置注入：

```C#
builder.Services.AddHangfire(opt =>
{
    opt.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)//兼容性等级
    .UseColouredConsoleLogProvider()//日志组件
    .UseSimpleAssemblyNameTypeSerializer()//序列化器组件
    .UseRecommendedSerializerSettings()//全局序列化器
    //存储介质SqlServer
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"),
    new Hangfire.SqlServer.SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true,
    })
    .UseTagsWithSql()
    .UseConsole(new ConsoleOptions { BackgroundColor = "#00079"})
    ;
});
builder.Services.AddHangfireServer();

//配置文件
"ConnectionStrings": {
    "HangfireConnection": "Data Source=LAPTOP-AVFER53P\\SQLEXPRESS;Initial Catalog=HangFireDemo;User ID=sa;Password=123456;TrustServerCertificate=true"
  }
```

#### 第三步，Use：

```C#
// 该方法废弃了，但不用还不行【坑】
app.UseHangfireServer();
//Hangfire 后台方法
HangfireMethod.BackMethodShow();

//延迟方法
HangfireMethod.DelayMethodShow();

//周期性任务
HangfireMethod.CycleMethodShow();
```

#### 唯一的优势 Dashboard：

```C#
    <PackageReference Include="Hangfire.Dashboard.BasicAuthorization" Version="1.0.2" />
```

```C#
app.UseHangfireDashboard("/hangfire");//支持Hangfire的可视化面板
app.UseHangfireDashboard("/Steven/hangfireNew");//支持Hangfire的可视化面板 
```

#### 封装扩展：

```C#
//builder.Services.AddHangfire(opt =>
//{
//    opt.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)//兼容性等级
//    .UseColouredConsoleLogProvider()//日志组件
//    .UseSimpleAssemblyNameTypeSerializer()//序列化器组件
//    .UseRecommendedSerializerSettings()//全局序列化器
//    //存储介质SqlServer
//    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"),
//    new Hangfire.SqlServer.SqlServerStorageOptions
//    {
//        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
//        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
//        QueuePollInterval = TimeSpan.Zero,
//        UseRecommendedIsolationLevel = true,
//        UsePageLocksOnDequeue = true,
//        DisableGlobalLocks = true,
//    })
//    .UseTagsWithSql()
//    .UseConsole(new ConsoleOptions { BackgroundColor = "#00079"})
//    ;
//});
//builder.Services.AddHangfireServer();

//改为这么一句话
builder.CustomAddHangfire();
```

#### Dashboard面板授权：

```C#
//支持可视化面板
//app.UseHangfireDashboard("/hangfire");//支持Hangfire的可视化面板
//app.UseHangfireDashboard("/Steven/hangfireNew");//支持Hangfire的可视化面板 
app.UseCustomHangfire();
```

```C#
//具体配置
//鉴权授权离不开他俩
app.UseAuthentication(); //鉴权
app.UseAuthorization();//授权
// Program 增加注入控制
#region Auth
//配置Cookies授权
builder.Services
 .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option => option.LoginPath = "/Account/Login");

//配置授权流程
builder.Services.AddAuthorization();
#endregion
    
```

面板配置项：

```C#
// 支持可视化面板
//app.UseHangfireDashboard("/hangfire");//支持Hangfire的可视化面板
//app.UseHangfireDashboard("/Steven/hangfireNew");//支持Hangfire的可视化面板 
app.UseCustomHangfire();
------------------------------------------------------------------------------------
//基础
app.UseHangfireDashboard("/hangfire");
// 可视化面板的授权--集成ASP.NET Core授权
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    //启用所有的权限
    IsReadOnlyFunc = (DashboardContext context) => true,
    //配置权限
    Authorization = new CustomDashboardAuthorizationFilter[]{new CustomDashboardAuthorizationFilter()},
    DashboardTitle = "Steven定时作业-Hangfire"
});
```

使用自定义Filter完成自定义逻辑控制:

```C#
//Filter
// 继承 IDashboardAuthorizationFilter
public class CustomDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    /// <summary>
    /// 这里就是用来验证权限 
    /// 可以集成asp.net core cookies授权
    /// </summary>
    /// <param name="context"></param>
    /// <returns>如果为true 表示验证通过了  false 验证不通过的</returns> 
    public bool Authorize([NotNull] DashboardContext context)
    {
        HttpContext httpContext = context.GetHttpContext();

        bool IsAuthenticated = httpContext.User.Identity?.IsAuthenticated ?? false;

        if (!IsAuthenticated)
        {
            httpContext.Response.Redirect("/Account/Login");
            return false;
        }
        return true;
    }
}

// 增加授权操作
 [HttpPost(Name = "GetWeatherForecast")]
        public string Login()
        {
            var claims = new List<Claim>
            {
                new Claim( ClaimTypes.Role,"Admin"),
                new Claim( ClaimTypes.Name,"Steven"),
                new Claim( "This guy very handsome?","true"),
                new Claim( "This guy very nice?","true"),
            };
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Steven"));
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(15),
            });
            return "OK";
        }
```

自定义鉴权授权：

```C#
// Program先增加读取配置
builder.Services.Configure<User>(builder.Configuration.GetSection("User"));

// hangfire自定义授权
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
    Authorization = new BasicAuthAuthorizationFilter[] {
        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            SslRedirect=false,
            RequireSsl=false,
            Users=userList
        })
    }
});
//appsettings
 "User": {
    "BasicAuthAuthorizationUser": [
      {
        "Name": "admin",
        "Password": "123456"
      },
      {
        "Name": "Steven",
        "Password": "123456"
      }
    ]
  }
```

---

#### 支持HttpJob：

```C#
// Add Hangfire services 
// 支持HttpJob:
//1.nuget引入:Hangfire.HttpJob
//2. .UseHangfireHttpJob
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
    .UseConsole(new ConsoleOptions{BackgroundColor = "#000079"})
     //主要是增加了这里这部分
    .UseHangfireHttpJob()); //支持httpJob
//GlobalConfiguration.Configuration.UseActivator(new AspNetCoreJobActivator());
// Add the processing server as IHostedService
builder.Services.AddHangfireServer(); //Hangfire的服务也添加到容器中去
```

```C#
//配置HTTPJob页面
{
  "RecurringJobIdentifier": "本次接口测试",
  "Url": "http://localhost:5041/WeatherForecast",
  "Method": "GET",
  "Data": "",
  "ContentType": "application/json",
  "Timeout": 5000,
  "DelayFromMinutes": 15,
  "Cron": "* * * * *",
  "JobName": "本次接口测试",
  "QueueName": "default",
  "AgentClass": "",
  "AgentTimeout": 0,
  "SendSuccess": false,
  "SendFail": true,
  "Mail": "",
  "EnableRetry": false,
  "RetryDelaysInSeconds": "20,30,60",
  "RetryTimes": 35,
  "BasicUserName": "",
  "BasicPassword": "",
  "Headers": {},
  "CallbackEL": "",
  "TimeZone": ""
}
```

