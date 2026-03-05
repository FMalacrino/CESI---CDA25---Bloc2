using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Security.Data.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        [NotMapped] // Pas dans la base de données
        public bool IsFavorite { get; set; }

        [JsonIgnore] // Pour ne pas transmettre la propriété via l'API
        public string UserId { get; set; } = ""; // string car la PK de IdentityUser est un string
    }
}