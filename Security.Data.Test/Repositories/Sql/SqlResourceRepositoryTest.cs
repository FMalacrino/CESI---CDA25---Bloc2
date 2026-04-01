using Security.Data.Repositories.Sql;

namespace Security.Data.Test.Repositories.Sql
{
    public class SqlResourceRepositoryTest
    {
        // Penser à mettre la configuration en TEST
        [Fact]
        public async Task GetOne()
        {
            int id = 1;
            var context = new DataContext();
            SqlResourceRepository repo = new SqlResourceRepository(context);


            var entity = await repo.GetOne(id);

            Assert.NotNull(entity);
            Assert.Equal(id, entity.Id);
        }
    }
}