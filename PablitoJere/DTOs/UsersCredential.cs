using System.ComponentModel.DataAnnotations;

namespace PablitoJere.DTOs
{
    public class UsersCredential
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

