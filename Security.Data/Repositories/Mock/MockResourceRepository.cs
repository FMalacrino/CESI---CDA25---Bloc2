using Security.Data.Models;

namespace Security.Data.Repositories.Mock
{
    public class MockResourceRepository : IResourceRepository
    {
        public async Task<Resource> Create(Resource model)
        {
            return await Task.FromResult(model);
        }

        public async Task<IEnumerable<Resource>> GetForUser(string? userId = null)
        {
            List<Resource> resources = [];
            for (int i = 1; i < 30; i++)
                resources.Add(new Resource()
                {
                    Id = i,
                    Name = $"Ressource {i}",
                    IsFavorite = i % 2 == 0,
                });

            return await Task.FromResult(resources);
        }

        public async Task<Resource?> GetOne(int id)
        {
            return await Task.FromResult(new Resource()
            {
                Id = id,
                Name = "Name"
            });
        }

        public async Task<Resource> Update(int id, Resource model)
        {
            return await Task.FromResult(new Resource()
            {
                Id = id,
                Name = model.Name,
                IsFavorite = model.IsFavorite,
            });
        }
    }
}