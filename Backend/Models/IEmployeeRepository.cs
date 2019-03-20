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

        Employee AddEmployee(Employee employee);
        void DeleteEmployee(int id);

        void AddVacation(Employee employee, Vacation vacation);
        void DeleteVacation(Employee employee, int id);
    }
}
