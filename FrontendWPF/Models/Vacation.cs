using System;

namespace FrontendWPF.Models
{
    public class Vacation
    {
        public int VacationId { get; set; }
        public DateTime Start { get; set; }
        public int Duration { get; set; }                
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }        
    }
}
