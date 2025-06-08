using Microsoft.AspNetCore.Mvc;
using tutorias.Models;

namespace tutorias.Features.Authentication
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
