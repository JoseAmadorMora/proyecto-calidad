using tutorias.Models;
using tutorias.Backend.Authentication;

namespace tutorias.Backend.Authentication
{
    public class AuthenticationLogic
    {
        private readonly IAuthenticationRepository authenticationRepository;

        public AuthenticationLogic(IAuthenticationRepository authenticationRepository)
        {
            this.authenticationRepository = authenticationRepository;
        }

        public UserModel? login(UserModel user)
        {
            return authenticationRepository.login(user);
        }
    }
}
