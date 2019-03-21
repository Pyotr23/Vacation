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

        //// GET api/values
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
