using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Steven.HangFire.Core.Utility;

//自定义 Hangfire 仪表板的访问权限  重写下面的方法
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
