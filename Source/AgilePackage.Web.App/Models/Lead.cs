namespace AgilePackage.Web.App.Models
{
    public class Lead : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public Lead(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
