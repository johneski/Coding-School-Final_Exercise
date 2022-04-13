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
    public class CustomerRepo : IEntityRepo<Customer>
    {
        private readonly FuelStationContext _fuelStationContext;

        public CustomerRepo(FuelStationContext fuelStationContext)
        {
            _fuelStationContext = fuelStationContext;
        }
        public async Task CreateAsync(Customer entity)
        {
            await _fuelStationContext.Customers.AddAsync(entity);
            await _fuelStationContext.SaveChangesAsync();
            
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _fuelStationContext.Customers.FindAsync(id);
            if (customer is not null)
            {
                customer.IsActive = false;
                await _fuelStationContext.SaveChangesAsync();
            }
            else
                throw new KeyNotFoundException("Customer not found");
        }

        public async Task<List<Customer>> GetAllActiveAsync()
        {
            return await _fuelStationContext.Customers.AsNoTracking().Where(x => x.IsActive).ToListAsync();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _fuelStationContext.Customers.AsNoTracking().ToListAsync();
        }

        public async Task<List<Customer>> GetAllInactiveAsync()
        {
            return await _fuelStationContext.Customers.AsNoTracking().Where(x => !x.IsActive).ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(Guid id, bool active)
        {
            return await _fuelStationContext.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == active && x.Id == id);
        }

        public async Task UpdateAsync(Guid id, Customer entity)
        {
            var customer = await _fuelStationContext.Customers.FindAsync(id);
            if(customer is not null)
            {
                customer.Name = entity.Name;
                customer.Surname = entity.Surname;
                customer.CardNumber = entity.CardNumber;
                customer.IsActive = entity.IsActive;
                await _fuelStationContext.SaveChangesAsync();
                return;
            }

            throw new KeyNotFoundException("Customer not found");
        }
    }
}
