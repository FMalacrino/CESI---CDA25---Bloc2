namespace Security.Data.Repositories.Mock
{
    public class MockAuthenticationRepository : IAuthenticationRepository
    {
        public async Task<bool> Login(string email, string password)
        {
            return await Task.FromResult(true);
        }

        public async Task Register(string email, string password)
        {
            await Task.CompletedTask;
        }
    }
}