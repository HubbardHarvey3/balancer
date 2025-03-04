using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Balancer.Components.Models
{
    internal class DonationEntryModel
    {
        [Key]
        public Guid TransactionId { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        public int DonorNumber { get; set; }

        public decimal Cash { get; set; }

        public decimal Check { get; set; }

        public decimal Total { get; set; }

        [Required]
        [Column(TypeName = "Date")]
        public DateOnly Date { get; set; }
    }
}
