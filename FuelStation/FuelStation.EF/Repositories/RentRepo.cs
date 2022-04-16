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
    public class RentRepo
    {
        private readonly FuelStationContext _context;

        public RentRepo(FuelStationContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Rent rent)
        {
            await _context.Rent.AddAsync(rent);
            await _context.SaveChangesAsync();
        }

        public async Task<Rent> GetByDateAsync(DateTime date)
        {
            return await _context.Rent.AsNoTracking().SingleOrDefaultAsync(x => x.Date.Year == date.Year && x.Date.Month == date.Month);
        }
        public async Task UpdateAsync(Guid id, Rent entity)
        {
            var rent = await _context.Rent.SingleOrDefaultAsync(x => x.Id == id);
            if(rent is not null)
            {
                rent.Value = entity.Value;
                rent.Date = entity.Date;
                await _context.SaveChangesAsync();
                return;
            }
        }
    }
}
