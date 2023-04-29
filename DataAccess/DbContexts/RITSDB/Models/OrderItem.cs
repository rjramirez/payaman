using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB.Models
{
    [Table("OrderItem")]
    public partial class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public short ProductId { get; set; }
        public short Quantity { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        [StringLength(128)]
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        [Required]
        [StringLength(128)]
        public string ModifiedBy { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderItems")]
        public virtual Order Order { get; set; }
        [ForeignKey("ProductId")]
        [InverseProperty("OrderItems")]
        public virtual Product Product { get; set; }
    }
}
