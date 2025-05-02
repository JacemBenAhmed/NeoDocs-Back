using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestBurOrdAPI.Models
{
    public class Demande
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime DateSoummision { get; set; }
        public Statuses Statut { get; set; } = Statuses.EnAttente;
        public ICollection<Documents> Documents { get; } = new List<Documents>();

        [Required]  
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }



    }
}
