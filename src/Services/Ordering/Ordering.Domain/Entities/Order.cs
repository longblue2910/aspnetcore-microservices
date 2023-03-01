using Contracts.Domains;
using Ordering.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain.Entities
{
    public class Order : EntityAuditBas<long>
    {
        [Required]
        public string UserName { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(250)")]
        public string EmailAddress { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string ShippingAddress { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string InvoiceAddress { get; set; }

        public OrderStatus Status { get; set; }
    }
}
