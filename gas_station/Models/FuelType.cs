using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class FuelType : Parent
    {
        [Display(Name = "Название типа топлива")]
        public string Name { get; set; }
        public List<Fuel> Fuels { get; set; } = new List<Fuel>();
        public List<FillingColumn> FillingColumns { get; set; } = new List<FillingColumn>();
    }
}
