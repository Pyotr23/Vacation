using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            if (!context.Employees.Any())
            {
                context.Employees.AddRange(
                    new Employee { Name = "Pyotr", Color = "Green" },
                    new Employee { Name = "Sergey", Color = "Blue" },
                    new Employee { Name = "Vasiliy", Color = "Red" }
                    );

                context.SaveChanges();

                context.Vacations.AddRange(
                    new Vacation { Start = new DateTime(2019, 5, 6), Duration = 7, Employee = context.Employees.Skip(0).FirstOrDefault() },
                    new Vacation { Start = new DateTime(2019, 3, 11), Duration = 7, Employee = context.Employees.Skip(0).FirstOrDefault() },
                    new Vacation { Start = new DateTime(2019, 5, 1), Duration = 28, Employee = context.Employees.Skip(1).FirstOrDefault() },
                    new Vacation { Start = new DateTime(2019, 4, 30), Duration = 7, Employee = context.Employees.Skip(2).FirstOrDefault() },
                    new Vacation { Start = new DateTime(2019, 5, 12), Duration = 7, Employee = context.Employees.Skip(2).FirstOrDefault() },
                    new Vacation { Start = new DateTime(2019, 5, 25), Duration = 7, Employee = context.Employees.Skip(2).FirstOrDefault() }
                    );

                context.SaveChanges();
            }
        }
    }
}
