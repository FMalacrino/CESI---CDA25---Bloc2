using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Security.Data.Repositories.Http
{
    public class HttpRepository
    {
        public static string Mail = "", Password = "";

        private readonly HttpClient client = new HttpClient();
        private readonly string apiUrl = "https://localhost:7084/api/"; //TODO Config
        private string token = "";

        public async Task<T?> Send<T>(HttpMethod method, string path, T? model = null) where T : class // nécessaire à cause de l'argument par défaut
        {
            // Appel quelconque Construction du la requete
            HttpRequestMessage request = new()
            {
                Method = method,
                RequestUri = new Uri(apiUrl + path)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Ajout du content si nécessaire
            if (model != null)
                request.Content = JsonContent.Create(model);

            // Envoi de la requete
            var response = client.Send(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await Login();
                return await Send<T>(method, path, model);
            }

            // TODO décider comment gérer les exception
            if (!response.IsSuccessStatusCode)
                throw new Exception("Requete invalide " + (int)response.StatusCode);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        private async Task Login()
        {
            var response = await client.PostAsJsonAsync(apiUrl + "Authentication/login", new { Email = Mail, Password = Password });
            if (!response.IsSuccessStatusCode)
                throw new Exception("Login failed " + (int)response.StatusCode);
            string json = await response.Content.ReadAsStringAsync();
            var jwt = DeserializeAnonymous(json, new { token = "", expiration = DateTime.Now });
            token = jwt.token;
        }

        private T? DeserializeAnonymous<T>(string json, T _)
          => JsonSerializer.Deserialize<T>(json);
    }
}