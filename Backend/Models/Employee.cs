using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }        
        public List<Vacation> Vacations { get; set; }
        public int ColorId { get; set; }
        [JsonIgnore]
        public Color Color { get; set; }
    }
}
