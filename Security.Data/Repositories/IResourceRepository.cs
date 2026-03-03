using Security.Data.Models;

namespace Security.Data.Repositories
{
    // Possibilité d'opération CRUD sur le modèle Resource
    public interface IResourceRepository
    {
        Task<Resource> Create(Resource model);

        Task<Resource?> GetOne(int id);

        Task<IEnumerable<Resource>> GetForUser(string? userId = null);
    }
}