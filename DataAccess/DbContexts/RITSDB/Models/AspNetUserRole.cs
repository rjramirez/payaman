using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB.Models
{
    [Index("RoleId", Name = "IX_AspNetUserRoles_RoleId")]
    public partial class AspNetUserRole
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("AspNetUserRoles")]
        public virtual AspNetRole Role { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("AspNetUserRoles")]
        public virtual AspNetUser User { get; set; }
    }
}
