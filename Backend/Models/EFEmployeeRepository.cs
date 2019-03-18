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

        public IQueryable<Employee> Employees => context.Employees.Include(v => v.Vacations);

        public IQueryable<Vacation> Vacations => context.Vacations;

        public void AddEmployee(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
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
    }
}
