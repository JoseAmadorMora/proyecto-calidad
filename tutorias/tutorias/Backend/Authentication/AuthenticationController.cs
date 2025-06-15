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
                ViewBag.UserId = userData.Id;
                ViewBag.UserName = userData.Name;
                ViewBag.UserEmail = userData.Email;
                var meta = "<meta charset=\"UTF-8\">";
                if (userData.UserType == UserTypes.Student)
                {
                    return Content($"{meta}<h1>Bienvenido {userData.Name}. Página principal de estudiantes se desarrollará posteriormente</h1>", "text/html");
                }
                else if (userData.UserType == UserTypes.Teacher)
                {
                    return Content($"{meta}<h1>Bienvenido {userData.Name}. Página principal de tutores se desarrollará posteriormente</h1>", "text/html");
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
