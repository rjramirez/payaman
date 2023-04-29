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
        public short Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(20)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public short RoleId { get; set; }
        public bool Active { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("AppUsers")]
        public virtual AppUserRole Role { get; set; }
        [InverseProperty("Cashier")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
