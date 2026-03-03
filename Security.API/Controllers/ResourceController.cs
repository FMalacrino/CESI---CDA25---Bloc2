using System.Security.Claims;
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
        private readonly UserManager<User> userManager;

        public ResourceController(IResourceRepository repository, UserManager<User> userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
        }

        [HttpGet("{id}")] // api/resource/id
        [AllowAnonymous]
        public async Task<ActionResult<Resource>> GetOne(int id)
        {
            var entity = await repository.GetOne(id);
            if (entity == null)
                return NotFound();
            return entity;
        }

        [HttpPost] // api/resource
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Resource>> Create(Resource model)
        {
            // Récupération utilisateur authentifié
            string? id = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            model.UserId = id;

            var entity = await repository.Create(model);
            return Ok(entity);
        }
    }
}