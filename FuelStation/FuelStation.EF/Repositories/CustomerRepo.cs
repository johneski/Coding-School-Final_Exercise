using FuelStation.EF.Context;
using FuelStation.EF.Models;
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
        public Task CreateAsync(Customer entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Customer>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Customer> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid id, Customer entity)
        {
            throw new NotImplementedException();
        }
    }
}
