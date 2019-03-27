using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Backend.Models
{
    public class Vacation
    {
        public int VacationId { get; set; }
        public DateTime Start { get; set; }
        public int Duration { get; set; }        
        //public DateTime Finish { get; set; }        
        public int EmployeeId { get; set; }
        [JsonIgnore]
        public Employee Employee { get; set; }

        //public Vacation()
        //{
        //    Finish = Start.AddDays(Duration);
        //}
    }
}
