using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Security.Data.Models;
using Security.Data.Repositories;

namespace Security.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceRepository repository;
        private readonly IUserManager userManager;

        public ResourceController(IResourceRepository repository, IUserManager userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
        }

        [HttpGet] // api/resource
        [Authorize(Roles = "User")]
        public async Task<ActionResult<List<Resource>>> GetForUser()
        {
            // Récupération utilisateur authentifié
            var id = userManager.GetUserId(User);
            if (id == null)
                return BadRequest();
            return Ok((await repository.GetForUser(id)).ToList());
        }

        [HttpGet("{id}")] // api/resource/id
        [AllowAnonymous]
        public async Task<ActionResult<Resource>> GetOne(int id)
        {
            var entity = await repository.GetOne(id);
            if (entity == null)
                return NotFound(entity);
            return Ok(entity);
        }

        [HttpPost] // api/resource
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Resource>> Create(Resource model)
        {
            // Récupération utilisateur authentifié
            var id = userManager.GetUserId(User);
            if (id == null)
                return BadRequest();

            model.UserId = id;
            var entity = await repository.Create(model);
            return Ok(entity);
        }
    }
}