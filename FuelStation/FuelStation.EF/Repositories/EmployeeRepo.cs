using FuelStation.EF.Context;
using FuelStation.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Repositories
{
    public class EmployeeRepo : IEntityRepo<Employee>
    {
        private readonly FuelStationContext _fuelStationContext;

        public EmployeeRepo(FuelStationContext fuelStationContext)
        {
            _fuelStationContext = fuelStationContext;
        }
        public async Task CreateAsync(Employee entity)
        {
            await _fuelStationContext.Employees.AddAsync(entity);
            await _fuelStationContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _fuelStationContext.Employees.FindAsync(id);
            if (employee is not null)
            {
                employee.IsActive = false;
                employee.HireDateEnd ??= DateTime.Now;
                await _fuelStationContext.SaveChangesAsync();
            }
            else
                throw new KeyNotFoundException("Employee not found");
        }

        public async Task<List<Employee>> GetAllActiveAsync()
        {
            return await _fuelStationContext.Employees.AsNoTracking().Include(x => x.Credentials).Where(x => x.IsActive).ToListAsync();
        }

        public async Task<List<Employee>> GetAllInactiveAsync()
        {
            return await _fuelStationContext.Employees.AsNoTracking().Where(x => !x.IsActive).ToListAsync();
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _fuelStationContext.Employees.AsNoTracking().ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(Guid id, bool active)
        {
            return await _fuelStationContext.Employees.AsNoTracking().Include(x => x.Credentials).SingleOrDefaultAsync(x => x.IsActive == active && x.Id == id);
        }

        public async Task UpdateAsync(Guid id, Employee entity)
        {
            var employee = await _fuelStationContext.Employees.Include(x => x.Credentials).SingleOrDefaultAsync(x => x.Id == id);
            if (employee is not null)
            {
                employee.Name = entity.Name;
                employee.Surname = entity.Surname;
                employee.HireDateStart = entity.HireDateStart;
                employee.HireDateEnd = entity.HireDateEnd;
                employee.Credentials.UserName = entity.Credentials.UserName;
                employee.Credentials.Password = entity.Credentials.Password;
                employee.SalaryPerMonth = entity.SalaryPerMonth;
                employee.EmployeeType = entity.EmployeeType;
                employee.IsActive = entity.IsActive;
                await _fuelStationContext.SaveChangesAsync();
                return;
            }

            throw new KeyNotFoundException("Employee not found");
        }

        public async Task LogOut(Guid id)
        {
            var credentials = await _fuelStationContext.UserCredentials.SingleOrDefaultAsync(x => x.AuthenticationToken == id);
            if(credentials is not null)
            {
                credentials.IsLogged = false;
                credentials.AuthenticationToken = null;
                await _fuelStationContext.SaveChangesAsync();
                return;
            }

            throw new KeyNotFoundException("Employee not found");
        }

        public async Task LogIn(Guid id)
        {
            var credentials = await _fuelStationContext.UserCredentials.SingleOrDefaultAsync(x => x.EmployeeId == id);
            if (credentials is not null)
            {
                credentials.IsLogged = true;
                await _fuelStationContext.SaveChangesAsync();
                return;
            }

            throw new KeyNotFoundException("Employee not found");
        }
    }
}
