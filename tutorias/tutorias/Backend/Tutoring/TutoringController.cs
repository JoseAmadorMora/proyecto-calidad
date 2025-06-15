using Microsoft.AspNetCore.Mvc;
using tutorias.Models;
using tutorias.Backend.Tutoring;

namespace tutorias.Backend.Tutorias
{
    public class TutoringController : Controller
    {
        private readonly TutoringLogic tutoringLogic;

        private void SetUserContext()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var userType = HttpContext.Session.GetInt32("UserType") ?? 0;

            ViewBag.UserId = userId;
            ViewBag.UserType = userType;
            ViewBag.IsProfessor = (userType == (int)UserTypes.Teacher);
        }

        public TutoringController(TutoringLogic tutoringLogic)
        {
            this.tutoringLogic = tutoringLogic;
        }
        public IActionResult TutoringMain()
        {
            SetUserContext();
            var allTutorships = tutoringLogic.GetAllTutorships();

            return View(allTutorships);
        }

        [HttpGet]
        public IActionResult SearchTutorships(string? course = null, string? sede = null, string? school = null)
        {
            SetUserContext();
            var tutorships = tutoringLogic.SearchTutorships(course, sede, school);

            return View("TutoringMain", tutorships);
        }

        [HttpGet]
        public IActionResult AddTutorship()
        {
            ViewData["Title"] = "Agregar Tutoria";
            return View("TutorshipForm", new TutoringModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTutorship(TutoringModel tutorship)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Unauthorized();

            tutorship.ProfessorId = userId.Value;

            bool success = tutoringLogic.AddTutorship(tutorship);
            if (success) {               
                return RedirectToAction("TutoringMain");
            }                 
            else
                return BadRequest("Error al crear tutoria");
        }

        [HttpGet]
        public IActionResult EditTutorship(int id)
        {
            var tutorship = tutoringLogic.GetTutorshipById(id);
            if (tutorship == null) return NotFound();

            ViewData["Title"] = "Editar Tutoria";
            return View("TutorshipForm", tutorship);
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
                return RedirectToAction("TutoringMain");
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
                return RedirectToAction("TutoringMain");
            else
                return BadRequest("Error al borrar tutoria");
        }
    }

}
