using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Security.API.Data.Models;

namespace Security.API.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        // provider: récupérer un service
        public static async Task Initialize(IServiceProvider provider)
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var context = provider.GetRequiredService<DataContext>();

            // Ajout des rôles
            string[] roles = configuration.GetSection("Roles").Get<string[]>();
            foreach (var item in roles)
            {
                if (!context.Roles.Any(r => r.Name == item))
                {
                    var identityRole = new IdentityRole(item)
                    {
                        NormalizedName = item.ToUpper()
                    };
                    var roleStore = new RoleStore<IdentityRole>(context);
                    await roleStore.CreateAsync(identityRole);
                }
            }

            // Ajout admin
            var adminRole = configuration.GetValue<string>("Admin:Role");
            var adminMail = configuration.GetValue<string>("Admin:Email");
            var adminName = configuration.GetValue<string>("Admin:Username");
            var adminPass = configuration.GetValue<string>("Admin:Password");
            //TODO Vérifier que les configs existent, sinon exception
            if(!context.Users.Any(u=> u.UserName == adminName))
            {
                var admin = new User
                {
                    Email = adminMail,
                    NormalizedEmail = adminMail?.ToUpper(),
                    UserName = adminName,
                    NormalizedUserName = adminName?.ToUpper(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                };

                // Mot de passe
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(admin, adminPass);
                admin.PasswordHash = hashed;

                // Ajout utilisateur
                var userStore = new UserStore<User>(context);
                await userStore.CreateAsync(admin);

                // Ajout rôle(s)
                UserManager<User> manager = provider.GetRequiredService<UserManager<User>>();
                var user = await manager.FindByIdAsync(admin.Id);
                await manager.AddToRolesAsync(user, [adminRole]);
            }

            await context.SaveChangesAsync();
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}