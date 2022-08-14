using System.ComponentModel.DataAnnotations;

namespace PablitoJere.Entities
{
    public class PublicationImage
    {
        public int Id { get; set; }

        public int PublicationId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string ImageUrl { get; set; }

        public Publication Publication { get; set; }

    }
}
