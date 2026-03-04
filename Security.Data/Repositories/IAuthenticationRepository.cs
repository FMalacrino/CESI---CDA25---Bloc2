namespace Security.Data.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<bool> Login(string email, string password);
        Task Register(string email, string password);
    }
}