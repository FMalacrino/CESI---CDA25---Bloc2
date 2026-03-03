using Microsoft.EntityFrameworkCore;
using Security.Data.Models;

namespace Security.Data.Repositories.Sql
{
    public class SqlResourceRepository : IResourceRepository
    {
        private readonly DataContext context;

        public SqlResourceRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<Resource> Create(Resource model)
        {
            context.Resources.Add(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<Resource?> GetOne(int id)
        {
            return await context.Resources
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}