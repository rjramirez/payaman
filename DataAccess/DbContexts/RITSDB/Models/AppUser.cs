using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB.Models
{
    [Table("AppUser")]
    public partial class AppUser
    {
        public AppUser()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public short AppUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(20)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public short AppUserRoleId { get; set; }
        public bool Active { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [StringLength(128)]
        public string CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }
        [StringLength(128)]
        public string ModifiedBy { get; set; }

        [ForeignKey("AppUserRoleId")]
        [InverseProperty("AppUsers")]
        public virtual AppUserRole AppUserRole { get; set; }
        [InverseProperty("Cashier")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
