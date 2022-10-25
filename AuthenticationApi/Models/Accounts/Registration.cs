﻿using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Models.Accounts
{
    public class Registration
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Second Name is required")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
