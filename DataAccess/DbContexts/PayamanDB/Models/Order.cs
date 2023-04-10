using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.PayamanDB.Models
{
    [Table("Order")]
    public partial class Order
    {
        [Key]
        [Column("OrderID")]
        public short OrderId { get; set; }
        [Column("ProductID")]
        public short ProductId { get; set; }
        public int Quantity { get; set; }
        [Required]
        [Column("CashierID")]
        [StringLength(128)]
        public string CashierId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        [Required]
        [StringLength(128)]
        public string ModifiedBy { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("Orders")]
        public virtual Product Product { get; set; }
    }
}
