using System.ComponentModel.DataAnnotations;

namespace PablitoJere.DTOs
{
    public class PublicationImageDTO
    {
        public int Id { get; set; }
        public int PublicationId { get; set; }

        [Required]
        public string ImageSrc { get; set; }
    }
}
