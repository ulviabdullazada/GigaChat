using GigaChat.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GigaChat.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            if (!Authorize()) return RedirectToAction(nameof(Register));
            var a = User;
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string name, string img)
        {
            if (Helper.Users.Any(x => x.Name == name))
                return NotFound();
            var newUser = new Models.User
            {
                Name = name,
                Image = img,
                Id = Guid.NewGuid()
            };
            await AddClaims(newUser.Id.ToString(), newUser.Name, newUser.Image);
            HttpContext.Session.SetString("user", newUser.Id.ToString());
            Helper.Users.Add(newUser);
            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public IActionResult Room()
        {
            return View();
        }
        private async Task AddClaims(string id, string name, string image)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, id));
            claims.Add(new Claim(ClaimTypes.Name, name));
            claims.Add(new Claim("image", image));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );
        }
        private bool Authorize()
        {
            if (!HttpContext.Session.Keys.Any(x => x == "user"))
            {
                return false;
            }
            return true;
        }
    }
}
