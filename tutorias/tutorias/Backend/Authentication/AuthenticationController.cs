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
        public IActionResult Login(UserModel user)
        {
            var userData = authenticationLogic.login(user);
            if (userData != null)
            {
                ViewBag["UserId"] = userData.Id;
                ViewBag["UserName"] = userData.Name;
                ViewBag["UserEmail"] = userData.Email;
                if (userData.UserType == UserTypes.Student)
                {
                    return Content("<h1>Página principal de estudiantes se desarollará posteriormente</h1>");
                }
                else if (userData.UserType == UserTypes.Teacher)
                {
                    return Content("<h1>Página principal de tutores se desarollará posteriormente</h1>");
                }
                else
                {
                    TempData["LoginError"] = "Error de servidor";
                    return RedirectToAction("LoginPage");
                }
            }
            else
            {
                TempData["LoginError"] = "usuario o contraseña incorrectos o inexistentes";
                return RedirectToAction("LoginPage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterUser(UserModel user)
        {
            var userId = authenticationLogic.RegisterUser(user);
            if (userId > 0)
            {
                return RedirectToAction("Login", user);
            }
            else
            {
                TempData["RegisterError"] = "No se pudo registrar el usuario";
                return RedirectToAction("LoginPage");
            }
        }
    }
}
