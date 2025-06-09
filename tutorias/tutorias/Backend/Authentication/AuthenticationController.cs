using Microsoft.AspNetCore.Mvc;
using tutorias.Backend.Authentication;
using tutorias.Models;

namespace tutorias.Features.Authentication
{
    public class AuthenticationController : Controller
    {
        private readonly AuthenticationLogic authenticationLogic;
        public AuthenticationController(AuthenticationLogic authenticationLogic)
        {
            this.authenticationLogic = authenticationLogic;
        }

        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserModel user)
        {
            var userData = authenticationLogic.login(user);
            if (userData != null)
            {
                // return ...
            }
            else
            {
                TempData["LoginError"] = "usuario o contraseña incorrectos o inexistentes";
                return RedirectToAction("LoginPage");
            }
        }
    }
}
