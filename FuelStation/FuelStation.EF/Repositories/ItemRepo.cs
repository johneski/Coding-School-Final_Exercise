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
    public class ItemRepo : IEntityRepo<Item>
    {
        private readonly FuelStationContext _fuelStationContext;

        public ItemRepo(FuelStationContext fuelStationContext)
        {
            _fuelStationContext = fuelStationContext;
        }

        public async Task CreateAsync(Item entity)
        {
            await _fuelStationContext.Items.AddAsync(entity);
            await _fuelStationContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _fuelStationContext.Items.FindAsync(id);
            if(item is not null)
            {
                item.IsActive = false;
                await _fuelStationContext.SaveChangesAsync();
                return;
            }

            throw new KeyNotFoundException("Item not found");
        }

        public async Task<List<Item>> GetAllActiveAsync()
        {
            return await _fuelStationContext.Items.AsNoTracking().Where(x => x.IsActive).ToListAsync();
        }

        public async Task<List<Item>> GetAllInactiveAsync()
        {
            return await _fuelStationContext.Items.AsNoTracking().Where(x => !x.IsActive).ToListAsync();
        }

        public async Task<List<Item>> GetAllAsync()
        {
            return await _fuelStationContext.Items.AsNoTracking().ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(Guid id, bool active)
        {
            return await _fuelStationContext.Items.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.IsActive == active);
        }

        public Task UpdateAsync(Guid id, Item entity)
        {
            throw new NotImplementedException();
        }
    }
}
