using System.ComponentModel.DataAnnotations;

namespace PablitoJere.DTOs
{
    public class EditAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
