using Security.Data.Models;

namespace Security.Data.Repositories.Http
{
    public class HttpResourceRepository : HttpRepository, IResourceRepository
    {
        public Task<Resource> Create(Resource model)
        {
            throw new NotImplementedException();
        }

        public async Task<Resource?> GetOne(int id)
        {
            return await Send<Resource>(HttpMethod.Get, $"resource/{id}");
        }
    }
}