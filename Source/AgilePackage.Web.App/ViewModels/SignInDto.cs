﻿namespace AgilePackage.Web.App.ViewModels
{
    public class SignInDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Invite { get; set; } = false;
    }
}
