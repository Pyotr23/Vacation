using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IEmployeeRepository employeeRepository;

        public ValuesController(IEmployeeRepository repo)
        {
            employeeRepository = repo;
        }        

        public IEnumerable<Employee> GetEmployees()
        {
            return employeeRepository.Employees;
        }

        [HttpGet("colors")]
        public IEnumerable<Color> GetColors()
        {
            return employeeRepository.Colors;
        }

        [HttpPost]
        public Employee PostEmployee([FromBody]Employee employee)
        {
            return employeeRepository.AddEmployee(employee);
        }

        [HttpDelete("{id}")]
        public void DeleteEmployee(int id)
        {
            employeeRepository.DeleteEmployee(id);
        }

        [HttpPost("vacation")]
        public void AddVacation([FromBody]VacationViewModel vacationVM)
        {
            employeeRepository.AddVacation(vacationVM.EmployeeId, vacationVM.Vacation);
        }

        [HttpDelete("{idE}/{idV}")]
        public void DeleteVacation(int idE, int idV)
        {
            employeeRepository.DeleteVacation(idE, idV);
        }
    }
}
