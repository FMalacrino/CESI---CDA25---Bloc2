using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Data.Repositories.Mock
{
    public class MockAuthenticationRepository : IAuthenticationRepository
    {
        public async Task<bool> Login(string email, string password)
        {
            throw new Exception();
            return await Task.FromResult(true);
        }
    }
}