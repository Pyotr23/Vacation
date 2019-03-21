using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class VacationViewModel
    {
        public int EmployeeId { get; set; }
        public Vacation Vacation { get; set; }
    }
}
