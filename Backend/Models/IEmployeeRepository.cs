using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> Employees { get; }
        IEnumerable<Vacation> Vacations { get; }

        void AddEmployee(Employee employee);
        void DeleteEmployee(int id);
    }
}
