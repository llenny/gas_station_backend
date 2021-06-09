using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class Fuel : Parent
    {
        [Display(Name = "Название топлива")]
        public string Name { get; set; }
        // тип топлива
        [Display(Name = "Тип топлива")]
        public int FuelTypeId { get; set; }
        public FuelType FuelType { get; set; }
        public List<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();

        //       [Display(Name = "Id записи в прайсе")]
        //       public int PriceListId { get; set; }
        public PriceList PriceList { get; set; }
        public Storage Storage { get; set; }
        //       public List<Storage> Storages { get; set; } = new List<Storage>();
    }
}
