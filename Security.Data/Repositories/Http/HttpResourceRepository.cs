using Security.Data.Models;

namespace Security.Data.Repositories.Http
{
    public class HttpResourceRepository : HttpRepository, IResourceRepository
    {
        public async Task<Resource> Create(Resource model)
        {
            return await Send(HttpMethod.Post, "resource", model);
        }

        public async Task<Resource?> GetOne(int id)
        {
            return await Send<Resource>(HttpMethod.Get, $"resource/{id}");
        }
    }
}