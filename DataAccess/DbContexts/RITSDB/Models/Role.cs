using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB.Models
{
    [Table("Role")]
    public partial class Role
    {
        public Role()
        {
            AppUserRoles = new HashSet<AppUserRole>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(10)]
        public string Name { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<AppUserRole> AppUserRoles { get; set; }
    }
}
