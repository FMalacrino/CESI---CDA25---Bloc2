using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Security.API.Controllers;
using Security.Data.Models;
using Security.Data.Repositories;

namespace Security.API.Test.Controllers
{
    public class ResourceControllerTest
    {
        [Theory]
        [InlineData(1, 200)]
        [InlineData(0, 404)]
        public async Task GetOneOkTest(int id, int expected)
        {
            IResourceRepository mockrepo = new MockResourceRepository();
            var controller = new ResourceController(mockrepo, null);

            ActionResult<Resource> result = await controller.GetOne(id);

            Assert.NotNull(result);
            var s = Assert.IsAssignableFrom<IStatusCodeActionResult>(result.Result);
            Assert.NotNull(s);
            Assert.Equal(expected, s.StatusCode);

            // Pas très propre mais évite de faire 2 facts
            if (expected == 200)
            {
                var r = (s as OkObjectResult)?.Value as Resource;
                Assert.NotNull(r);
                Assert.Equal(id, r.Id);
            }
        }

        [Fact]
        public async Task GetForUserOkTest()
        {
            int expected = 200;
            IResourceRepository mockrepo = new MockResourceRepository();
            IUserManager mockusermanager = new MockUserManager();
            var controller = new ResourceController(mockrepo, mockusermanager);

            var result = await controller.GetForUser(); // . Result var tâche asynchrone

            Assert.NotNull(result);
            var s = result.Result as OkObjectResult;
            //Assert.IsType<StatusCodeResult>(result.Result);
            Assert.NotNull(s);
            Assert.Equal(expected, s.StatusCode);
            var r = s.Value as IEnumerable<Resource>;
            Assert.NotNull(r);
        }
    }

    public class MockResourceRepository : IResourceRepository
    {
        public Task<Resource> Create(Resource model)
        {
            return null;
        }

        public async Task<IEnumerable<Resource>> GetForUser(string? userId = null)
        {
            return await Task.FromResult(new List<Resource>());
        }

        public async Task<Resource?> GetOne(int id)
        {
            return
                await Task.FromResult(
                id != 0
                ? new Resource() { Id = id }
                : null);
        }

        public async Task<Resource> Update(int id, Resource model)
        {
            return null;
        }
    }

    public class MockUserManager : IUserManager
    {
        public string? GetUserId(ClaimsPrincipal principal)
        {
            return "id";
        }
    }
}