using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class User :  IdentityUser<int>
    {
        [Display(Name = "Пароль")]
        public virtual string Password { get; set; }
 //       public List<Order> Orders { get; set; } = new List<Order>();
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int IdentityRoleId { get; set; }
        public IdentityRole<int> IdentityRole { get; set; }
        public Employee Employee { get; set; }
    }
}
