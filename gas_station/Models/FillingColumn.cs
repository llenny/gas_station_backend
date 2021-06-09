using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class FillingColumn : Parent
    {
        [Display(Name = "Код колонки")]
        public string Code { get; set; }
        [Display(Name = "Тип топлива")]
        // к каждой колонке привязаны определенные типы топлива
        public List<FuelType> FuelTypes { get; set; } = new List<FuelType>();
        // каждая колонка привязана к одной из станций
        public int StationId { get; set; }
        public Station Station { get; set; }
    }
}
