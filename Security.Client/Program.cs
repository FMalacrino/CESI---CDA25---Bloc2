using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Security.Data.Repositories.Http;

namespace Security.Client
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private const string url = "https://localhost:7084/api/"; // url du server API

        static async Task Main(string[] args)
        {
            Thread.Sleep(500); // Pour laisser le temps au serveur de démarrer

            HttpRepository.Mail = "r@r.r";
            HttpRepository.Password = "Azerty1234*";

            var repo = new HttpResourceRepository();
            try
            {
                var model = repo.GetOne(1).Result;
                Console.WriteLine(model.Name);

                for (int i = 0; i < 10; i++)
                {
                    model = new Data.Models.Resource() { Name = "Bar" };
                    model = await repo.Create(model);
                    Console.WriteLine(model.Id);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message + " " + ex.GetType().ToString());
                Console.ResetColor();
            }
            Console.ReadKey();
            return;

            try
            {
                RegisterUser();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message + " " + ex.GetType().ToString());
                Console.ResetColor();
            }
            Console.ReadKey();
            return;

            HttpResponseMessage response;

            // HttpRequestMessage -> HttpResponseMessage

            try
            {
                //TODO Demander mail et pass à la console
                // Register + login

                // login
                string loginUrl = url + "Authentication/login";
                var body = new { Email = "admin@mail.com", Password = "secret" };
                response = client.PostAsJsonAsync(loginUrl, body).Result; // Créer la reuqte avec body en json
                // .Result: je veux exécuter la méthode de manière synchrone

                //TODO traiter cas d'erreur
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Authentification échec");

                LoginResponseModel? jwt = response.Content.ReadFromJsonAsync<LoginResponseModel>().Result;
                Console.WriteLine(jwt.token + " " + jwt.expiration.ToString("g"));

                //string json = response.Content.ReadAsStringAsync().Result;
                //var jwt = DeserializeAnonymous(json, new { token = "", expiration = DateTime.Now });
                //Console.WriteLine(jwt.token);

                // Appel quelconque Construction du la requete
                HttpRequestMessage request = new()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url + "test")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.token);

                // Envoi de la requete
                response = client.Send(request);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Requete invalide " + (int)response.StatusCode);
                // 401 si token incorrect ou expiré 403 si role incorrect
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK !!!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message + " " + ex.GetType().ToString());
                Console.ResetColor();
            }
            Console.ReadKey();
        }

        public static void RegisterUser()
        {
            Console.WriteLine("Enter your email :");
            string? mail = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(mail))
                throw new Exception("Empty email");

            Console.WriteLine("Enter your password :");
            string? password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Empty password");

            var response = client.PostAsJsonAsync(url + "Authentication/register", new { Email = mail, Password = password }).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception("Registration failed " + (int)response.StatusCode);
            Console.WriteLine("Registration success");

            response = client.PostAsJsonAsync(url + "Authentication/login", new { Email = mail, Password = password }).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception("Login failed " + (int)response.StatusCode);
            string json = response.Content.ReadAsStringAsync().Result;
            var jwt = DeserializeAnonymous(json, new { token = "", expiration = DateTime.Now });
            Console.WriteLine("Login until " + jwt.expiration.ToString("g"));
        }

        public static T? DeserializeAnonymous<T>(string json, T _)
            => JsonSerializer.Deserialize<T>(json);

        public class LoginResponseModel
        {
            public string token { get; set; } = "";
            public DateTime expiration { get; set; }
        }
    }
}