using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB.Models
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        [Key]
        public short ProductId { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        [StringLength(50)]
        public string Image { get; set; }
        public short StoreId { get; set; }
        public bool Active { get; set; }
        [Precision(4)]
        public DateTime CreatedDate { get; set; }
        [Required]
        [StringLength(128)]
        public string CreatedBy { get; set; }
        [Precision(4)]
        public DateTime? ModifiedDate { get; set; }
        [StringLength(128)]
        public string ModifiedBy { get; set; }

        [ForeignKey("StoreId")]
        [InverseProperty("Products")]
        public virtual Store Store { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
