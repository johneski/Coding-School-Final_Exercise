using FuelStation.EF.Handlers;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using FuelStation.Blazor.Shared.ViewModels;

namespace FuelStation.Blazor.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEntityRepo<Employee> _employeeRepo;
        private readonly UserValidation _userValidation;
        private readonly DataValidation _dataValidation;

        public EmployeeController(IEntityRepo<Employee> employeeRepo, UserValidation validation)
        {
            _employeeRepo = employeeRepo;
            _userValidation = validation;
        }

        [HttpGet("active")]
        public async Task<IEnumerable<EmployeeViewModel>> GetAllActive([FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var employees = await _employeeRepo.GetAllActiveAsync();
                return employees.Select(x => new EmployeeViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    EmployeeType = x.EmployeeType,
                    HireDateEnd = x.HireDateEnd,
                    HireDateStart = x.HireDateStart,
                    SalaryPerMonth = x.SalaryPerMonth
                });
            }
            return new List<EmployeeViewModel>();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromHeader] Guid authToken, EmployeeViewModel employee)
        {
            if (await _userValidation.ValidateTokenAsync(authToken) && _dataValidation.Validate(employee))
            {
                var newEmployee = new Employee()
                {
                    Name = employee.Name,
                    Surname = employee.Surname,
                    EmployeeType = employee.EmployeeType,
                    HireDateEnd = employee.HireDateEnd,
                    HireDateStart = employee.HireDateStart,
                    SalaryPerMonth = employee.SalaryPerMonth
                };

                await _employeeRepo.CreateAsync(newEmployee);
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("active/{id}")]
        public async Task<EmployeeViewModel> GetActiveEmployee([FromRoute] Guid id, Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var employee = await _employeeRepo.GetByIdAsync(id, true);
                if (employee is not null)
                {
                    return new EmployeeViewModel()
                    {
                        Id = employee.Id,
                        Name = employee.Name,
                        Surname = employee.Surname,
                        EmployeeType = employee.EmployeeType,
                        HireDateEnd = employee.HireDateEnd,
                        HireDateStart = employee.HireDateStart,
                        SalaryPerMonth = employee.SalaryPerMonth
                    };
                }
            }

            return new EmployeeViewModel();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                try
                {
                    await _employeeRepo.DeleteAsync(id);
                    return Ok();
                }
                catch (KeyNotFoundException ex)
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }

        [HttpGet("inactive")]
        public async Task<IEnumerable<EmployeeViewModel>> GetAllInactive([FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var employees = await _employeeRepo.GetAllInactiveAsync();
                return employees.Select(x => new EmployeeViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    EmployeeType = x.EmployeeType,
                    HireDateEnd = x.HireDateEnd,
                    HireDateStart = x.HireDateStart,
                    SalaryPerMonth = x.SalaryPerMonth
                });
            }
            return new List<EmployeeViewModel>();
        }

        [HttpPut("undo/{id}")]
        public async Task<IActionResult> Undo([FromRoute] Guid id, [FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var employee = await _employeeRepo.GetByIdAsync(id, false);
                if (employee is not null)
                {
                    employee.IsActive = true;
                    await _employeeRepo.UpdateAsync(employee.Id, employee);
                    return Ok();
                }
            }

            return BadRequest();
        }
    }
}
