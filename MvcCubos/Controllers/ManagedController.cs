using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MvcCubos.Services;

namespace MvcCubos.Controllers
{
    public class ManagedController : Controller
    {

        private ServiceApiCubos service;
        private ServiceStorageBlobs serviceStorageBlobs;

        public ManagedController(ServiceApiCubos service, ServiceStorageBlobs serviceStorageBlobs)
        {
            this.service = service;
            this.serviceStorageBlobs = serviceStorageBlobs;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            string token = await this.service.GetTokenAsync(email, password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
            else
            {
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                Claim claimUserName = new Claim(ClaimTypes.Name, email);
                Claim claimRole = new Claim(ClaimTypes.Role, "USUARIO");
                identity.AddClaim(claimUserName);
                identity.AddClaim(claimRole);
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                //INTRODUCIMOS AL USUARIO EN EL SISTEMA
                await HttpContext.SignInAsync
                    (
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddMinutes(15)
                    });
                return RedirectToAction("Index", "Cubos");

            }
        }


        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("Index", "Cubos");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, IFormFile file, string password)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceStorageBlobs.UploadBlobAsync("cubosprivate", blobName, stream);
            }

            await this.service.RegisterUser(nombre, email, blobName, password);
            return RedirectToAction("Login");
        }
    }
}
