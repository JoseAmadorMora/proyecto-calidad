using Microsoft.AspNetCore.Mvc;
using tutorias.Models;
using tutorias.Backend.Tutoring;

namespace tutorias.Backend.Tutorias
{
    public class TutoringController : Controller
    {
        private readonly TutoringLogic tutoringLogic;

        public TutoringController(TutoringLogic tutoringLogic)
        {
            this.tutoringLogic = tutoringLogic;
        }
        public IActionResult Main()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var userType = HttpContext.Session.GetInt32("UserType") ?? 0;

            var allTutorships = tutoringLogic.GetAllTutorships();

            ViewBag.UserId = userId;
            ViewBag.UserType = userType;
            ViewBag.IsProfessor = (userType == (int)UserTypes.Teacher);

            return View(allTutorships);
        }

        [HttpGet]
        public IActionResult SearchTutorships(string? course = null, string? sede = null, string? school = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var userType = HttpContext.Session.GetInt32("UserType") ?? 0;

            ViewBag.IsProfessor = (userType == (int)UserTypes.Teacher);
            ViewBag.UserId = userId;

            var tutorships = tutoringLogic.SearchTutorships(course, sede, school);

            return View("Main", tutorships);
        }

        [HttpGet]
        public IActionResult AddTutorship()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTutorship(TutoringModel tutorship)
        {
            bool success = tutoringLogic.AddTutorship(tutorship);
            if (success)
                return RedirectToAction("Main");
            else
                return BadRequest("Error al crear tutoria");
        }

        [HttpGet]
        public IActionResult EditTutorship(int id)
        {
            var tutorship = tutoringLogic.GetTutorshipById(id);
            return View(tutorship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTutorship(TutoringModel tutorship)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != tutorship.ProfessorId)
                return Forbid();

            bool success = tutoringLogic.UpdateTutorship(tutorship);
            if (success)
                return RedirectToAction("Main");
            else
                return BadRequest("Error al actualizar tutoria");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTutorship(int id, int professorId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var tutorship = tutoringLogic.GetTutorshipById(id);

            if (tutorship == null || tutorship.ProfessorId != userId)
                return Forbid();

            bool success = tutoringLogic.DeleteTutorship(id, professorId);
            if (success)
                return RedirectToAction("Main");
            else
                return BadRequest("Error al borrar tutoria");
        }
    }

}
