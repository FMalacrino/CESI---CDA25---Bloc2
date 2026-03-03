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

                model = new Data.Models.Resource() { Name = "Bar" };
                model = await repo.Create(model);
                Console.WriteLine(model.Id);

                Console.WriteLine(string.Join(',', (await repo.GetForUser()).Select(x => x.Id)));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message + " " + ex.GetType().ToString());
                Console.ResetColor();
            }
            Console.ReadKey();

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