using FuelStation.EF.Context;
using FuelStation.EF.Handlers;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelStation.Blazor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        private readonly UserValidation _userValidation;
        private readonly FuelStationContext _context;

        public ValidationController(UserValidation userValidation, FuelStationContext context)
        {
            _userValidation = userValidation;
            _context = context;
        }

        [HttpPost]
        public async Task<Guid?> ValidateUser([FromHeader] string username,[FromHeader] string password)
        {
            var employee = await _userValidation.ValidateUserAsync(username, password);
            if (employee is not null)
            {
                var credentials = await _context.UserCredentials.SingleOrDefaultAsync(x => x.EmployeeId == employee.Id);
                credentials.AuthenticationToken = await _userValidation.CreateTokenAsync(employee.Id);
                credentials.IsLogged = true;
                await _context.SaveChangesAsync();
                return credentials.AuthenticationToken;
            }

            return Guid.Empty;
        }

        
    }
}
