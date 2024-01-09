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
//���ö�ȡhangfire��Ȩ
builder.Services.Configure<User>(builder.Configuration.GetSection("User"));
//����Cookies��Ȩ
builder.Services
 .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option => option.LoginPath = "/Account/Login");
//������Ȩ����
builder.Services.AddAuthorization();
#endregion

//����Hangfire
builder.CustomAddHangfire();
var app = builder.Build();
app.UseAuthentication(); //��Ȩ
app.UseAuthorization();//��Ȩ
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//һ��Ҫ�Ӳ�֪��Ϊʲô ��Ȼ˵�Ѿ�����
app.UseHangfireServer();
//֧�ֿ��ӻ����
app.UseCustomHangfire();
app.MapControllers();
app.Run();
