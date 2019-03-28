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
                context.Colors.AddRange(
                    new Color { ColorNumber = -16777216 },  // чёрный
                    new Color { ColorNumber = -16744448 },  // зелёный
                    new Color { ColorNumber = -16776961 },  // синий
                    new Color { ColorNumber = -65536 },     // красный
                    new Color { ColorNumber = -256 },       // жёлтый
                    new Color { ColorNumber = -8388480 },   // фиолетовый
                    new Color { ColorNumber = -23296 },     // оранжевый
                    new Color { ColorNumber = -8355712 },   // серый
                    new Color { ColorNumber = -16711936 },  // лайм
                    new Color { ColorNumber = -16181 }      // розовый
                    );

                context.SaveChanges();

                context.Employees.AddRange(
                    new Employee { Name = "Pyotr", Color = context.Colors.Skip(0).FirstOrDefault() },
                    new Employee { Name = "Sergey", Color = context.Colors.Skip(1).FirstOrDefault() },
                    new Employee { Name = "Vasiliy", Color = context.Colors.Skip(2).FirstOrDefault() }
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
