using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class Storage : Parent
    {
        [Display(Name = "Единица топлива")]
        public int FuelId { get; set; }
        public Fuel Fuel { get; set; }
        //public List<Fuel> Fuels { get; set; } = new List<Fuel>();
        [Display(Name = "Имеющийся объем")]
        public float Value { get; set; }
        [Display(Name = "Статус")]
        public bool Status { get; set; }
        // на каждой станции определенный резерв каждого вида топлива
        public int StationId { get; set; }
        public Station Station { get; set; }
    }
}
