
using Security.Data.Repositories;
using Security.Data.Repositories.Mock;

namespace Security.Mobile.Services
{
    public class DataService
    {
        public readonly IAuthenticationRepository authenticationRepository = new MockAuthenticationRepository();
    }
}