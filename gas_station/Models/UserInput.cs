using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class UserInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int EmployeeId { get; set; }
    }
}
