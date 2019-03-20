using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class EFEmployeeRepository : IEmployeeRepository
    {
        private ApplicationDbContext context;

        public EFEmployeeRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Employee> Employees => context.Employees.Include(v => v.Vacations);

        public IEnumerable<Vacation> Vacations => context.Vacations;

        public Employee AddEmployee(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public void DeleteEmployee(int id)
        {
            Employee deletedEmployee = context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (deletedEmployee != null)
            {
                context.Employees.Remove(deletedEmployee);
                context.SaveChanges();
            }
        }

        public Vacation AddVacation(Employee employee, Vacation vacation)
        {
            context.Employees
            context.Vacations.Add(vacation);
            context.SaveChanges();
            return vacation;
        }

        public void DeleteVacation(int id)
        {
            Vacation deletedVacation = context.Vacations.FirstOrDefault(v => v.VacationId == id);
            if (deletedVacation != null)
            {
                context.Vacations.Remove(deletedVacation);
                context.SaveChanges();
            }
        }
    }
}
