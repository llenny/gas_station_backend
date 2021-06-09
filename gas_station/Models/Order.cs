using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class Order : Parent
    {
        [Display(Name = "Создатель заказа")]
        public int UserId { get; set; }
        public User User { get; set; }
        [Display(Name = "Единица топлива")]
        public int FuelId { get; set; }
        public Fuel Fuel { get; set; }
        [Display(Name = "Объем")]
        public float Value { get; set; }
        [Display(Name = "Скидка")]
        public float Discount { get; set; }
        [Display(Name = "Цена")]
        public float Price { get; set; }
        [Display(Name = "Сумма")]
        public float Sum { get; set; }
        public int StationId { get; set; }
        public Station Station { get; set; }
    }
}
