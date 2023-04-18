﻿using System;
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
            AppUserRoles = new HashSet<AppUserRole>();
        }

        [Key]
        public int Id { get; set; }
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
        public bool? IsActive { get; set; }

        [InverseProperty("AppUser")]
        public virtual ICollection<AppUserRole> AppUserRoles { get; set; }
    }
}
