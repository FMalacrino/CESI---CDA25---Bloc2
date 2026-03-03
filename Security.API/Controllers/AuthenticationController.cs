using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Security.Data.Models;

namespace Security.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AuthenticationController(IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        [Route("login")] // On rajoute à l'url /login
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model) // FromBody: ne va chercher que dans le body
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            // Insertion dans le header de la réponse
            var authClaims = new List<Claim>()
            {
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.SerialNumber, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Ajout des roles
            var roles = await userManager.GetRolesAsync(user);
            foreach (var item in roles)
                authClaims.Add(new Claim(ClaimTypes.Role, item));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            // Génération du token
            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(1), // TODO dans le fichier de config
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return Ok(new // body de la réponse http
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Vérification existence utilisateur
            User? user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            // Ajout utilisateur
            user = new()
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            IdentityResult result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError);

            // Ajout rôle
            await userManager.AddToRoleAsync(user, "User"); //TODO assigner ce rôle par défaut dans le settings
            return Ok();
        }

        [HttpPost]
        [Route("registeradmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterModel model)
        {
            // Vérification existence utilisateur
            User? user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            // Ajout utilisateur
            user = new()
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            IdentityResult result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError);

            // Ajout rôle
            try
            {
                await userManager.AddToRoleAsync(user, model.Role);
                return Ok();
            }
            catch { return StatusCode(StatusCodes.Status400BadRequest); }
        }
    }

    public class LoginModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class AdminRegisterModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
    }
}