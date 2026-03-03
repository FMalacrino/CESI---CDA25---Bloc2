using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Security.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string url = "https://localhost:7084/api/"; // url du server API
            HttpClient client = new HttpClient();
            HttpResponseMessage response;

            // HttpRequestMessage -> HttpResponseMessage
            Thread.Sleep(500); // Attendre 2000ms pour laisser le temps au serveur de démarrer

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

                // Appel quelconque
                // Construction du la requete
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
                // 401 si token incorrect ou expiré
                // 403 si role incorrect
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

        public static T? DeserializeAnonymous<T>(string json, T _)
            => JsonSerializer.Deserialize<T>(json);

        public class LoginResponseModel
        {
            public string token { get; set; }
            public DateTime expiration { get; set; }
        }
    }
}