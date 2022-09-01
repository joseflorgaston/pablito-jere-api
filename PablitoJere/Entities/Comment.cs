using Microsoft.AspNetCore.Identity;

namespace PablitoJere.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public int PublicacionId { get; set; }
        public string UsuarioId { get; set; }
        public IdentityUser IdentityUser { get; set; }
    }
}
