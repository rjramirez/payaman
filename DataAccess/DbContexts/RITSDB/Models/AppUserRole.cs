using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB.Models
{
    [Table("AppUserRole")]
    public partial class AppUserRole
    {
        public AppUserRole()
        {
            AppUsers = new HashSet<AppUser>();
        }

        [Key]
        public short AppUserRoleId { get; set; }
        [Required]
        [StringLength(10)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Description { get; set; }

        [InverseProperty("AppUserRole")]
        public virtual ICollection<AppUser> AppUsers { get; set; }
    }
}
