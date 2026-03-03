using Microsoft.AspNetCore.Identity;

namespace Security.Data.Models
{
    // Représente un utilisateur authentifié dans la BdD
    public class User : IdentityUser
    {
        public virtual ICollection<Resource> Resources { get; set; } = [];
    }
}
