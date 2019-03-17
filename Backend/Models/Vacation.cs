using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Vacation
    {
        public int ProductId { get; set; }
        public DateTime Start { get; set; }
        public int Duration { get; set; }
        public DateTime Finish => Start.AddDays(Duration);

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
