using FuelStation.EF.Context;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Handlers
{
    public class UserValidation
    {
        private readonly FuelStationContext _context;

        public UserValidation(FuelStationContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateToken(Guid authToken)
        {
            var credentials = await _context.UserCredentials.AsNoTracking().SingleOrDefaultAsync(x => x.AuthenticationToken == authToken);
            if (credentials is null)
                return false;
            return true;
        }

        public async Task<Guid> CreateToken(Guid employeeId)
        {
            Guid authToken = Guid.NewGuid();
            var credentials = await _context.UserCredentials.SingleOrDefaultAsync(x => x.EmployeeId == employeeId);
            if (credentials is null)
                throw new KeyNotFoundException("There is no such Employee");

            credentials.AuthenticationToken = authToken;
            await _context.SaveChangesAsync();
            return authToken;
        }

        public async Task DeleteToken(Guid employeeId)
        {
            var credentials = await _context.UserCredentials.SingleOrDefaultAsync(x => x.EmployeeId == employeeId);
            if(credentials is null)
                throw new KeyNotFoundException("There is no such Employee");

            credentials.AuthenticationToken = null;
            await _context.SaveChangesAsync();
        }

        public async Task<Employee?> ValidateUser(string username, string password)
        {
            var credentials = await _context.UserCredentials.AsNoTracking().SingleOrDefaultAsync(x => x.UserName == username && x.Password == password);            
            if(credentials is not null)
                return await _context.Employees.AsNoTracking().SingleOrDefaultAsync(x => x.Id == credentials.EmployeeId);

            return null;
        }
    }
}
