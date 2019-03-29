using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendWPF.Models
{
    public class Color
    {
        public int ColorId { get; set; }
        public int ColorNumber { get; set; }
        public List<Employee> Employees { get; set; } 
    }
}
