namespace Security.Data.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public string UserId { get; set; } = ""; // string car la PK de IdentityUser est un string
    }
}