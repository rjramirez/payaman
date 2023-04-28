using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB.Models
{
    [Table("Order")]
    [Index("ProductId", Name = "IX_Order_ProductId")]
    public partial class Order
    {
        [Key]
        public short Id { get; set; }
        public short ProductId { get; set; }
        public int Quantity { get; set; }
        public short CashierId { get; set; }
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

        [ForeignKey("ProductId")]
        [InverseProperty("Orders")]
        public virtual Product Product { get; set; }
    }
}
