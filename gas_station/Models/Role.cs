using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class Role : IdentityRole<int>
    {
        public override string Name { get; set; }
    }
}
