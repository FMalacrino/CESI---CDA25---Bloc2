using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Security.Data.Repositories.Http;

namespace Security.API.TI
{
    public class ResourceTI
    {
        // Penser à mettre la configuration en DEBUG et à lancer l'API
        [Fact]
        public async Task GetOne()
        {
            int id = 1;
            HttpRepository.Mail = "admin@mail.com";
            HttpRepository.Password = "secret";

            HttpResourceRepository repo = new HttpResourceRepository();

            var result = await repo.GetOne(id);
            Assert.NotNull(result);
        }
    }
}
