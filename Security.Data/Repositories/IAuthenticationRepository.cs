using Security.Data.Models;

namespace Security.Data.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<bool> Login(Credential credential);

        Task Register(Credential credential);
    }
}