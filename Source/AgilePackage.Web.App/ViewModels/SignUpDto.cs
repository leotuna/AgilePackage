namespace AgilePackage.Web.App.ViewModels
{
    public class SignUpDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool Invite { get; set; } = false;
    }
}
