using FuelStation.EF.Handlers;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuelStation.Blazor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        private readonly UserValidation _userValidation;

        public ValidationController(UserValidation userValidation)
        {
            _userValidation = userValidation;
        }

        [HttpPost]
        public async Task<Guid> ValidateUser([FromHeader] string username,[FromHeader] string password)
        {
            var employee = await _userValidation.ValidateUserAsync(username, password);
            if (employee is not null)
            {                
                return await _userValidation.CreateTokenAsync(employee.Id);
            }

            return Guid.NewGuid();
        }

        
    }
}
