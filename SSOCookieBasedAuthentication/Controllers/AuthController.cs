using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SSOCookieBasedAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private const string USERNAME = "demo";
        private const string PASSWORD = "demo";

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!USERNAME.Equals(username) || !PASSWORD.Equals(password))
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("CustomClaim", "custom claim")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = false, // true : cookie will be deleted after expired. Even if the browser is closed, it will be exists. false : if browser is closed then cookie will be removed. It can be associated with remember me
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1)
                });

            return Ok();
        }


        [HttpGet]
        [Route("GetClaims")]
        public IActionResult GetClaims()
        {
            string? username = HttpContext.User.Identity?.Name;
            string? customClaim = HttpContext.User.FindFirst("CustomClaim")?.Value;

            List<string?> result = new List<string?>();
            result.Add(username);
            result.Add(customClaim);

            return Ok(result);
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }
    }
}
