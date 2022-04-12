using FuelStation.EF.Handlers;
using FuelStation.Blazor.Shared.Enums;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using FuelStation.Blazor.Shared.ViewModels;
using FuelStation.Blazor.Shared.Tools;

namespace FuelStation.Blazor.Server.Controllers
{    

    [ApiController]
    [Route("[controller]")]    
    public class CustomerController : ControllerBase
    {
        private readonly IEntityRepo<Customer> _customerRepo;
        private readonly UserValidation _userValidation;
        private readonly DataValidation _dataValidation;

        public CustomerController(IEntityRepo<Customer> customerRepo, UserValidation validation, DataValidation dataValidation)
        {
            _customerRepo = customerRepo;
            _userValidation = validation;
            _dataValidation = dataValidation;
        }

        [HttpGet("active")]
        public async Task<IEnumerable<CustomerViewModel>> GetAllActive([FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var customers = await _customerRepo.GetAllActiveAsync();
                return customers.Select(x => new CustomerViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    CardNumber = x.CardNumber,
                });
            }
            return new List<CustomerViewModel>();
        }

        [HttpGet("inactive")]
        public async Task<IEnumerable<CustomerViewModel>> GetAllInactive([FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var customers = await _customerRepo.GetAllInactiveAsync();
                return customers.Select(x => new CustomerViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    CardNumber = x.CardNumber,
                });
            }
            return new List<CustomerViewModel>();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromHeader] Guid authorization, CustomerViewModel customer)
        {
            if(await _userValidation.ValidateTokenAsync(authorization) && _dataValidation.Validate(customer))
            {
                var newCustomer = new Customer()
                {
                    Name = customer.Name,
                    Surname = customer.Surname,
                    CardNumber = customer.CardNumber,
                };

                await _customerRepo.CreateAsync(newCustomer);
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("active/{id}")]
        public async Task<CustomerViewModel> GetActiveCustomer([FromRoute] Guid id, [FromHeader]Guid authorization)
        {
            if(await _userValidation.ValidateTokenAsync(authorization))
            {
                var customer = await _customerRepo.GetByIdAsync(id, true);
                if (customer is not null)
                {
                    return new CustomerViewModel()
                    {
                        Id = customer.Id,
                        Name = customer.Name,
                        Surname = customer.Surname,
                        CardNumber = customer.CardNumber,
                    };
                }
            }

            return new CustomerViewModel();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                try
                {
                    await _customerRepo.DeleteAsync(id);
                    return Ok();
                }
                catch (KeyNotFoundException ex)
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromHeader] Guid authorization, [FromBody] CustomerViewModel customerView)
        {

            if(await _userValidation.ValidateTokenAsync(authorization))
            {
                try
                {
                    var customer = await _customerRepo.GetByIdAsync(customerView.Id, true);
                    if (customer is not null && _dataValidation.Validate(customerView))
                    {
                        customer.Name = customerView.Name;
                        customer.Surname = customerView.Surname;
                        customer.CardNumber = customerView.CardNumber;
                        await _customerRepo.UpdateAsync(customer.Id, customer);
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
            if(await _userValidation.ValidateTokenAsync(authorization))
            {
                var customer = await _customerRepo.GetByIdAsync(id, false);
                if(customer is not null)
                {
                    customer.IsActive = true;
                    await _customerRepo.UpdateAsync(customer.Id, customer);
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
                (employeeType == EmployeeType.Manager || employeeType == EmployeeType.Cashier))
            {
                return true;
            }

            return false;
        }

        [HttpGet("newcustomer")]
        public async Task<CustomerViewModel> NewCustomer()
        {
            Tools tools = new();
            CustomerViewModel customer = new();
            string cardNumber;
            var customers = await _customerRepo.GetAllAsync();
            var cardNumbersList = customers.Select(x => x.CardNumber).ToList();
            while (true)
            {
                cardNumber = tools.GenerateCardNumber();
                if (!cardNumbersList.Contains(cardNumber))
                    break;
            }
            customer.CardNumber = cardNumber;
            return customer;
        }
    }
}
