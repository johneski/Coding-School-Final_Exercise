using FuelStation.EF.Handlers;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using FuelStation.Blazor.Shared.ViewModels;

namespace FuelStation.Blazor.Server.Controllers
{    

    [ApiController]
    [Route("[controller]")]    
    public class CustomerController : ControllerBase
    {
        private readonly IEntityRepo<Customer> _customerRepo;
        private readonly UserValidation _validation;

        public CustomerController(IEntityRepo<Customer> customerRepo, UserValidation validation)
        {
            _customerRepo = customerRepo;
            _validation = validation;
        }

        [HttpGet("active")]
        public async Task<List<Customer>> GetAllActive([FromHeader] Guid authToken)
        {
            if (await _validation.ValidateToken(authToken))
            {
                return await _customerRepo.GetAllActiveAsync();
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromHeader] Guid authToken ,CustomerViewModel customer)
        {
            var dataValidation = new DataValidation();
            if(await _validation.ValidateToken(authToken) && dataValidation.Validate(customer))
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
    }
}
