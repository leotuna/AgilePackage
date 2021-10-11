namespace AgilePackage.Web.Models
{
    public class Contact
    {
        public string Email { get; set; }
        public string Message { get; set; }
        public string City { get; set; }
        public bool? Success { get; set; }

        public void WasNotSuccessful()
        {
            Success = false;
        }
    }
}
