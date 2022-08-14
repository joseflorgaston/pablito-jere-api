using PablitoJere.Entities;
using PablitoJere.Validations;
using System.ComponentModel.DataAnnotations;

namespace PablitoJere.DTOs
{
    public class PublicationCreateDTO
    {
        [Required(ErrorMessage = "This field is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "Title field maximum Lenght is 120 characters")]
        [Capitalized]
        public string Title { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(maximumLength: 2400, ErrorMessage = "Description field maximum Lenght is 2400 characters")]
        [Capitalized]
        public string Description { get; set; }

        public List<string> PublicationImages { get; set; }
    }
}
