# Sécurité ASP

## Mise en place

- Créer un nouveau projet API ASP.NET Core  **sans authentification** et avec **prise en charge de HTTPS**
- Supprimer `WeatherForecast` et `WeatherForecastController`
- Ajouter les paquet Nuget
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.Sqlite
  - Microsoft.EntityFrameworkCore.Tools
  - Microsoft.AspNetCore.Authentication.JwtBearer
- Créer un répertoire `Data`
- Créer un répertoire `Models` dans `Data`

## Base de données

- Dans le répertoire `Models` ajouter une classe `User`:

    ```cs
    public class User : IdentityUser
    {
    }
    ```

- Dans le répertoire `Data` ajouter une classe `DataContext`:

    ```cs
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
    ```

- Ajouter la chaine de connexion dans le fichier `appsettings.json` :

    ```json
    "ConnectionStrings": {
        "Default": "Data Source=data.db3"
    },
    ```

- Dans la classe `Program`, ajouter les lignes suivantes:

    ```cs
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
    builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<DataContext>()
        .AddDefaultTokenProviders();
    ```

- Dans la console de gestionnaire de paquet, ajouter la migration intiale : `dotnet ef migrations add Initial`
   > Si ça ne fonctionne pas, installer le paquet avec `dotnet tool install --global dotnet-ef`
- Puis créer la base de données : `dotnet ef database update`
- Vérifier la base de données créée

## Rôles et administrateur

- Dans le fichier `appsettings.json`, ajouter la définition des rôles et de l'admin :

    ```json
    "Roles" :  ["Admin","User"],
    "Admin": {
        "Username": "Admin",
        "Email": "admin@mail.com",
        "Password": "secret",
        "Role": "Admin"
    }
    ```

- Dans la classe `DataContext`, ajouter la création des rôles et de l'admin :

    ```cs
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var context = serviceProvider.GetRequiredService<DataContext>();

            // Ajout des roles
            string[] roles = configuration.GetSection("Roles").Get<string[]>();
            foreach (var item in roles)
                if (!context.Roles.Any(r => r.Name == item))
                {
                    var identityRole = new IdentityRole(item) { NormalizedName = item.ToUpper() }; // NormalizedName obligatoire
                    var roleStore = new RoleStore<IdentityRole>(context);
                    await roleStore.CreateAsync(identityRole);
                }

            // Ajout admin
            var adminRole = configuration.GetValue<string>("Admin:Role");
            var adminMail = configuration.GetValue<string>("Admin:Email");
            var adminName = configuration.GetValue<string>("Admin:Username");
            var adminPass = configuration.GetValue<string>("Admin:Password");
            if (!context.Users.Any(u => u.UserName == adminName))
            {
                
                var admin = new User
                {
                    Email = adminMail,
                    NormalizedEmail = adminMail?.ToUpper(),
                    UserName = adminName,
                    NormalizedUserName = adminName?.ToUpper(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };
                
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(admin, adminPass);
                admin.PasswordHash = hashed;

                var userStore = new UserStore<User>(context);
                await userStore.CreateAsync(admin);

                UserManager<User> manager = serviceProvider.GetRequiredService<UserManager<User>>();
                var user = await manager.FindByIdAsync(admin.Id);
                await manager.AddToRolesAsync(user, [adminRole]);
            }

            await context.SaveChangesAsync();
        }
    ```

- Dans la class Program, ajouter le seed:

    ```cs
    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        DataContext.Initialize(scope.ServiceProvider); 
    }
    ```

- Ajouter un contrôleur d'API vide `AuthenticationController` dans le dossier `Controllers`:

    ```cs
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
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            User? user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            var roles = await userManager.GetRolesAsync(user);
            return Ok(new { id = user.Id, roles = roles });
        }
    }

    public class LoginModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
    ```

- Dans un cmd, tester le compte admin avec `curl https://localhost:7156/api/authentication/login -i -X POST -H "Content-Type:application/json"  -d "{\"Email\": \"admin@mail.com\",  \"Password\" : \"secret\" }"`

## JSON Web Token

- Dans le fichier `appsettings.json`, ajouter la configuation JWT :

    ```json
    "JWT": {
        "Secret": "MotDePasseTresSecret1.0EtTresTresTresTresLongsjdggkUGIUGksdghsdfKUGJGV454354"
    }
    ```

- Dans le classe `Program`, ajouter la définition du JWT:

    ```cs
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey=true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });
    ```

- Puis l'utilisation de l'authentification

    ```cs
    app.UseAuthentication();
    app.UseAuthorization();
    ```

- Dans la méthode `Login` du controller `AuthenticationController`, ajouter le retour du JWT:

    ```cs
    var authClaims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

    var roles = await userManager.GetRolesAsync(user);
    foreach (var userRole in roles)
        authClaims.Add(new Claim(ClaimTypes.Role, userRole));

    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

    var token = new JwtSecurityToken(
        expires: DateTime.Now.AddHours(1),
        claims: authClaims,
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

    return Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token),
        expiration = token.ValidTo
    });
    ```

- Ajouter un controlleur d'API vide `TestController` avec le code :

    ```cs
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Get()
        {
            return Ok("Authorized");
        }
    }
    ```

- Obtenir le token en appellant l'url : `curl https://localhost:7156/api/authentication/login -i -X POST -H "Content-Type:application/json"  -d "{\"Email\": \"admin@mail.com\",  \"Password\" : \"secret\" }"`
- Tester sans le token: `curl https://localhost:7156/api/test -i -H "Content-Type:application/json"`
- Tester avec le token: `curl https://localhost:7156/api/test -i -H "Content-Type:application/json" -H "Authorization: Bearer $Token$"`

## Swagger

- Ajouter le paquet Nuget `Swashbuckle.AspNetCore`
- Dans la classe `Program`, remplacer `builder.Services.AddOpenApi();` par:

    ```cs
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

        // Add JWT Bearer Auth
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });
    ```

- Remplacer le code en environnement de développement par:

    ```cs
    app.UseSwagger();
    app.UseSwaggerUI();
    ```

- Exécuter le code et accéder à l'interface de Swagger via l'url `https://localhost:7156/swagger`
- Tester l'action `GET api/test`
    >>> 401 Unautorized
- Exécuter l'action `POST api/authentication/login` avec le json :

    ```json
    {
        "email": "admin@mail.com",
        "password": "secret"
    }
    ```

- Récupérer le jeton reçu et le mettre dans `Authorize` en haut de la page
- Tester à nouveau l'action `GET api/test`
    >>> 200 OK

## Client

- Créer un projet console pour tester l'API
- Remplacer la méthode `Main`par le code suivant:

    ```cs
        static void Main(string[] args)
        {
            const string url = "https://localhost:7156/api/";
            HttpClient client = new();
            HttpResponseMessage response;

            try
            {
                // Login
                var login = new { Email = "admin@mail.com", Password = "secret" };
                response = client.PostAsJsonAsync(url + "authentication/login", login).Result;
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Login error : {(int)response.StatusCode} {response.StatusCode}");

                // Token
                string json = response.Content.ReadAsStringAsync().Result;
                var jwt = DeserializeAnonymous(json, new { token = "", expiration = DateTime.Now });
                if (jwt == null)
                    throw new Exception("Empty token");
                Console.WriteLine("Token Received : \n" + jwt.token + "\n" + jwt.expiration);

                // Test
                HttpRequestMessage message = new()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url + "test")
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.token);
                response = client.Send(message);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Test error : {(int)response.StatusCode} {response.StatusCode}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Test ok");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        public static T? DeserializeAnonymous<T>(string json, T _)
            => JsonSerializer.Deserialize<T>(json);
    ```

- Exécuter l'API puis l'application console et vérifier le résultat

## Enregistrement

- Dans la classe `AuthenticationController`, ajouter le code suivant pour enregistrer un nouvel utilisateur et un nouvel admin:

    ```cs
            [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            User? user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            user = new ()
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result == null || !result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, result?.ToString());

            await userManager.AddToRoleAsync(user, "User");

            return Ok();
        }

        [HttpPost]
        [Route("register-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            User? user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            user = new()
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result == null || !result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, result?.ToString());

            await userManager.AddToRoleAsync(user, "Admin");

            return Ok();
        }

        public class RegisterModel
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }
    ```
