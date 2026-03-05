using Security.Data.Models;

namespace Security.Data.Repositories.Mock
{
    public class MockAuthenticationRepository : IAuthenticationRepository
    {
        public async Task<bool> Login(Credential credential)
        {
            return await Task.FromResult(true);
        }

        public async Task Register(Credential credential)
        {
            await Task.CompletedTask;
        }
    }
}