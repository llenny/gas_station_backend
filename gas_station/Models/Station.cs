using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class Station : Parent
    {

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Комментарий")]
        public string Note { get; set; }

        [Display(Name = "Статус")]
        public bool Status { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<FillingColumn> FillingColumns { get; set; } = new List<FillingColumn>();
        public List<Storage> Storages { get; set; } = new List<Storage>();
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
