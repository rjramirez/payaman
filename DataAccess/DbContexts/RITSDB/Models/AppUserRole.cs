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
        [Key]
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("AppUserId")]
        [InverseProperty("AppUserRoles")]
        public virtual AppUser AppUser { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("AppUserRoles")]
        public virtual Role Role { get; set; }
    }
}
