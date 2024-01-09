using Hangfire;
using Hangfire.Console;
using Hangfire.Tags.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Steven.HangFire.Core.Utility;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region Auth
//配置读取hangfire授权
builder.Services.Configure<User>(builder.Configuration.GetSection("User"));
//配置Cookies授权
builder.Services
 .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option => option.LoginPath = "/Account/Login");
//配置授权流程
builder.Services.AddAuthorization();
#endregion

//配置Hangfire
builder.CustomAddHangfire();
var app = builder.Build();
app.UseAuthentication(); //鉴权
app.UseAuthorization();//授权
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//一定要加不知道为什么 虽然说已经废弃
app.UseHangfireServer();
//支持可视化面板
app.UseCustomHangfire();
app.MapControllers();
app.Run();
