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
        IEnumerable<Color> Colors { get; }

        Employee AddEmployee(Employee employee);
        void DeleteEmployee(int id);

        void AddVacation(int employeeId, Vacation vacation);
        void DeleteVacation(int employeeId, int vacationId);
    }
}
