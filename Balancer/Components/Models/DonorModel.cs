using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Balancer.Components.Models
{
    internal class DonorModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DonorNumber { get; set; }

        public  string Name { get; set; }

        public decimal TotalDonations { get; set; }

        public  string Address { get; set; }
    }
}
