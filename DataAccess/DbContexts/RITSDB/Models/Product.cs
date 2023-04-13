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
            Orders = new HashSet<Order>();
        }

        [Key]
        [Column("ProductID")]
        public short ProductId { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Description { get; set; }
        [StringLength(10)]
        public string ImageName { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        [Required]
        [StringLength(128)]
        public string ModifiedBy { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
