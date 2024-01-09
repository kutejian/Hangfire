using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Steven.HangFire.Core;
using System.Security.Claims;

namespace Steven.HangFire.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "GetWeatherForecast")]
        public string Login()
        {
            var claims = new List<Claim>
            {
                new Claim( ClaimTypes.Role,"Admin"),
                new Claim( ClaimTypes.Name,"Steven"),
                new Claim( "password","123456"),
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
    }
}