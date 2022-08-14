using System.ComponentModel.DataAnnotations;

namespace PablitoJere.DTOs
{
    public class PublicationDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        public List<string> imageUrls { get; set; }

    }
}
