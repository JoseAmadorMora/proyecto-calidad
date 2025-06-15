using tutorias.Models;

namespace tutorias.Backend.Tutoring
{
    public class TutoringLogic
    {
        private readonly ITutoringRepository tutoringRepository;

        public TutoringLogic(ITutoringRepository tutorshipRepository)
        {
            this.tutoringRepository = tutorshipRepository;
        }

        public List<TutoringModel> GetAllTutorships()
        {
            return tutoringRepository.GetAllTutorships();
        }

        public TutoringModel? GetTutorshipById(int id)
        {
            return tutoringRepository.GetTutorshipById(id);
        }

        public List<TutoringModel> SearchTutorships(string? course, string? sede, string? school = null)
        {
            return tutoringRepository.SearchTutorships(course, sede, school);
        }

        public bool AddTutorship(TutoringModel tutorship)
        {
            return tutoringRepository.AddTutorship(tutorship);
        }

        public bool UpdateTutorship(TutoringModel tutorship)
        {
            return tutoringRepository.UpdateTutorship(tutorship);
        }

        public bool DeleteTutorship(int id, int professorId)
        {
            return tutoringRepository.DeleteTutorship(id, professorId);
        }

        public List<TutoringModel> GetTutorshipsByProfessorId(int professorId)
        {
            return tutoringRepository.GetTutorshipsByProfessorId(professorId);
        }
    }
}
