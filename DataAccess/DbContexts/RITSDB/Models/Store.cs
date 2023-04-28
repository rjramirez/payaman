using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB.Models
{
    [Table("Store")]
    public partial class Store
    {
        public Store()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public short Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Description { get; set; }
        [StringLength(10)]
        public string Image { get; set; }
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

        [InverseProperty("Store")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
