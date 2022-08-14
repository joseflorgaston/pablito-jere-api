using System.ComponentModel.DataAnnotations;

namespace PablitoJere.DTOs
{
    public class PublicationImageCreateDTO
    {
        public string ImageSrc { get; set; }
        public int PublicationId { get; set; }

    }
}
