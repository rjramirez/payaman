using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.PayamanDB.Models
{
    [Table("AppUser")]
    public partial class AppUser
    {
        [Key]
        [Column("AppUserID")]
        public short AppUserId { get; set; }
        [Required]
        [StringLength(30)]
        public string UserName { get; set; }
        public string Password { get; set; }
        [StringLength(100)]
        public string ImageFileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(128)]
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        [Required]
        [StringLength(128)]
        public string ModifiedBy { get; set; }
    }
}
