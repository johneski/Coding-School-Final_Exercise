using FuelStation.EF.Handlers;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using FuelStation.Blazor.Shared.ViewModels;
using FuelStation.Blazor.Shared.Enums;

namespace FuelStation.Blazor.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEntityRepo<Employee> _employeeRepo;
        private readonly UserValidation _userValidation;
        private readonly DataValidation _dataValidation;

        public EmployeeController(IEntityRepo<Employee> employeeRepo, UserValidation validation, DataValidation dataValidation)
        {
            _employeeRepo = employeeRepo;
            _userValidation = validation;
            _dataValidation = dataValidation;
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
        public async Task<IActionResult> CreateEmployee([FromHeader] Guid authorization, EmployeeViewModel employee)
        {
            if (await _userValidation.ValidateTokenAsync(authorization) && _dataValidation.Validate(employee))
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
                var credentials = new UserCredentials()
                {
                    EmployeeId = employee.Id,
                    UserName = employee.Username,
                    Password = employee.Password,
                };
                newEmployee.Credentials = credentials;
                try
                {
                    await _employeeRepo.CreateAsync(newEmployee);
                }
                catch (Exception ex)
                {
                    return BadRequest("There was a conflict");
                }
                
                return Ok();
            }

            return BadRequest("Wrong data inputs");
        }

        [HttpGet("active/{id}")]
        public async Task<EmployeeViewModel> GetActiveEmployee([FromRoute] Guid id, [FromHeader] Guid authorization)
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
                        Username = employee.Credentials.UserName,
                        Password = employee.Credentials.Password,
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

        [HttpPut]
        public async Task<IActionResult> Update([FromHeader] Guid authorization, [FromBody] EmployeeViewModel employeeView)
        {

            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                try
                {
                    var employee = await _employeeRepo.GetByIdAsync(employeeView.Id, true);
                    if (employee is not null && _dataValidation.Validate(employeeView))
                    {
                        employee.Name = employeeView.Name;
                        employee.Surname = employeeView.Surname;
                        employee.EmployeeType = employeeView.EmployeeType;
                        employee.Credentials.UserName = employeeView.Username;
                        employee.Credentials.Password = employeeView.Password;
                        employee.HireDateEnd = employeeView.HireDateEnd;
                        employee.HireDateStart = employeeView.HireDateStart;
                        employee.SalaryPerMonth = employeeView.SalaryPerMonth;
                        await _employeeRepo.UpdateAsync(employee.Id, employee);
                        return Ok();
                    }
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound();
                }
            }

            return BadRequest();

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

        [HttpGet("authorization")]
        public async Task<bool> EmployeeAuthorization([FromHeader] Guid authorization)
        {
            var employeeType = await _userValidation.GetEmployeeTypeAsync(authorization);
            if (employeeType is not null &&
                (employeeType == EmployeeType.Manager))
            {
                return true;
            }

            return false;
        }
    }
}
