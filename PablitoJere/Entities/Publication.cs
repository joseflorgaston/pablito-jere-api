using PablitoJere.Validations;
using System.ComponentModel.DataAnnotations;

namespace PablitoJere.Entities
{
    public class Publication
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "Title field maximum Length is 120 characters")]
        [Capitalized]
        public string Title { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(maximumLength: 1200, ErrorMessage = "Description field maximum Length is 1200 characters")]
        [Capitalized]
        public string Description { get; set; }

        public List<PublicationImage> PublicationImages { get; set; }

    }
}
