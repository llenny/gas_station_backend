using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class Employee : Man
    {
        [Display(Name = "Табельный номер")]
        public int Code { get; set; }
        // каждый сотрудник может быть привязан к нескольким станциям
        public List<Station> Stations { get; set; } = new List<Station>();
        // должность
        public int PositionId { get; set; }
        public Position Position { get; set; }
        public int ? UserId { get; set; }
        public User User { get; set; }
        public override DateTime CreateDate { get => base.CreateDate; set => base.CreateDate = value; }
        public override DateTime UpdateDate { get => base.UpdateDate; set => base.UpdateDate = value; }

    }
}
