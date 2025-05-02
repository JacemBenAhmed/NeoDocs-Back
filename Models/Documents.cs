using System.ComponentModel.DataAnnotations;

namespace GestBurOrdAPI.Models
{
    public class Documents
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Demande Demande { get; set; }
        [Required]
        public string NomFichier { get; set; }
        [Required]
        public DocumentTypes DocumentTypes { get; set; }
        [Required]
        public string Url { get; set; }
    }
}
