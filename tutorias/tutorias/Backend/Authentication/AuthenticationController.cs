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

        public IActionResult RegisterPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult login(UserModel user)
        {
            var userData = authenticationLogic.login(user);
            if (userData != null)
            {
                HttpContext.Session.SetInt32("UserId", userData.Id);
                HttpContext.Session.SetInt32("UserType", (int)userData.UserType);

                return RedirectToAction("TutoringMain", "Tutoring");
            }
            else
            {
                TempData["LoginError"] = "usuario o contrasena incorrectos o inexistentes";
                return RedirectToAction("LoginPage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult registerUser(UserModel user)
        {
            try
            {
                var userId = authenticationLogic.registerUser(user);
                if (userId > 0)
                {
                    return View("RedirectSuccesfulRegisterToAutoLogin", user);
                }
                else
                {
                    TempData["RegisterError"] = "No se pudo registrar el usuario";
                    return RedirectToAction("RegisterPage");
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["RegisterError"] = ex.Message;
                return RedirectToAction("RegisterPage");
            }
            catch
            {
                TempData["RegisterError"] = "No se pudo registrar el usuario";
                return RedirectToAction("RegisterPage");
            }
        }
    }
}
