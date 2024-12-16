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
        [Route("/Index/Home/{room=global}")]
        public IActionResult Index(string room)
        {
            if (!Authorize()) return RedirectToAction(nameof(Register));
            ViewBag.Room = room;
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string name, string img, string returnUrl)
        {
            if (Helper.Users.Any(x => x.Name == name))
                return RedirectToAction(nameof(Room));
            var newUser = new Models.User
            {
                Name = name,
                Image = img,
                Id = Guid.NewGuid()
            };
            await AddClaims(newUser.Id.ToString(), newUser.Name, newUser.Image);
            HttpContext.Session.SetString("user", newUser.Id.ToString());
            Helper.Users.Add(newUser);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction(nameof(Room));
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
