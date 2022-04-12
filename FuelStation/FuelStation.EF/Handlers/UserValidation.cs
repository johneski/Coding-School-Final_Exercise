using FuelStation.Blazor.Shared.Enums;
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

        public async Task<bool> ValidateTokenAsync(Guid authToken)
        {
            var credentials = await _context.UserCredentials.AsNoTracking().SingleOrDefaultAsync(x => x.AuthenticationToken == authToken);
            if (credentials is null)
                return false;
            return true;
        }

        public async Task<Guid> CreateTokenAsync(Guid employeeId)
        {
            Guid authToken = Guid.NewGuid();
            var credentials = await _context.UserCredentials.SingleOrDefaultAsync(x => x.EmployeeId == employeeId);
            if (credentials is null)
                throw new KeyNotFoundException("There is no such Employee");

            credentials.AuthenticationToken = authToken;
            await _context.SaveChangesAsync();
            return authToken;
        }

        public async Task DeleteTokenAsync(Guid employeeId)
        {
            var credentials = await _context.UserCredentials.SingleOrDefaultAsync(x => x.EmployeeId == employeeId);
            if(credentials is null)
                throw new KeyNotFoundException("There is no such Employee");

            credentials.AuthenticationToken = null;
            await _context.SaveChangesAsync();
        }

        public async Task<Employee?> ValidateUserAsync(string username, string password)
        {
            // The first to login gets registered as Admin
            if((await _context.Employees.ToListAsync()).Count() == 0)
            {
                var employee = new Employee()
                {
                    Name = "admin",
                    Surname = "admin",
                    EmployeeType = Blazor.Shared.Enums.EmployeeType.Manager
                };
                employee.Credentials = new UserCredentials()
                {
                    EmployeeId = employee.Id,
                    UserName = username,
                    Password = password,
                };
                _context.Employees.Add(employee);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                               
            }

            var credentials = await _context.UserCredentials.AsNoTracking().SingleOrDefaultAsync(x => x.UserName == username && x.Password == password);            
            if(credentials is not null)
                return await _context.Employees.AsNoTracking().SingleOrDefaultAsync(x => x.Id == credentials.EmployeeId && x.IsActive);

            return null;
        }

        public async Task<EmployeeType?> GetEmployeeTypeAsync(Guid authToken)
        {
            var employeeCredentials = await _context.UserCredentials.AsNoTracking().FirstOrDefaultAsync(x => x.AuthenticationToken == authToken);
            if(employeeCredentials is not null)
            {
                var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeCredentials.EmployeeId && x.IsActive);
                if (employee is not null)
                    return employee.EmployeeType;
            }
            return null;
        }
    }
}
