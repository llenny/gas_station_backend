using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class Delivery : Parent
    {

        [Display(Name = "Создатель заявки на поставку")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        
        [Display(Name = "Статус поставки (false - зарегистрирована, true - завершена)")]
        public bool Status { get; set; }

        [Display(Name = "Стоимость")]
        public int Sum { get; set; }

        [Display(Name = "Принял поставку")]
        public int ? EndBy { get; set; }

        [Display(Name = "Дата завершения")]
        public DateTime ? EndDate { get; set; }
        // на какую станцию будет поставка
        public int StationId { get; set; }
        public Station Station { get; set; }

        public List<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();

    }
}
