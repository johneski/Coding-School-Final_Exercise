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
    public class TransactionRepo : IEntityRepo<Transaction>
    {
        private readonly FuelStationContext _fuelStationContext;

        public TransactionRepo(FuelStationContext fuelStationContext)
        {
            _fuelStationContext = fuelStationContext;
        }

        public async Task CreateAsync(Transaction entity)
        {
            await _fuelStationContext.Transactions.AddAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var transaction = await _fuelStationContext.Transactions.FindAsync(id);
            if (transaction is not null)
            {
                transaction.IsActive = false;
                await _fuelStationContext.SaveChangesAsync();
                return;
            }

            throw new KeyNotFoundException("Transaction not found");
        }

        public async Task<List<Transaction>> GetAllActiveAsync()
        {
            return await _fuelStationContext.Transactions.AsNoTracking().Include(x => x.TransactionLines).Include(x => x.Customer).Include(x => x.Employee).Where(x => x.IsActive).ToListAsync();
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _fuelStationContext.Transactions.AsNoTracking().Include(x => x.TransactionLines).ToListAsync();
        }

        public async Task<List<Transaction>> GetAllInactiveAsync()
        {
            return await _fuelStationContext.Transactions.AsNoTracking().Where(x => !x.IsActive).ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(Guid id, bool active = true)
        {
            var transaction = await _fuelStationContext.Transactions.AsNoTracking().Include(x => x.TransactionLines).SingleOrDefaultAsync(x => x.Id == id);
            if(transaction is not null)
                return transaction;

            throw new KeyNotFoundException("Transaction not found");
        }

        public async Task UpdateAsync(Guid id, Transaction entity)
        {
            var transaction = await _fuelStationContext.Transactions.Include(x => x.TransactionLines).SingleOrDefaultAsync(x => x.Id == id);
            if(transaction is not null)
            {
                transaction.CustomerId = entity.CustomerId;
                transaction.EmployeeId = entity.EmployeeId;
                transaction.Date = entity.Date;
                transaction.PaymentMethod = entity.PaymentMethod;
                transaction.Total = entity.Total;
                transaction.TransactionLines = entity.TransactionLines;
                await _fuelStationContext.SaveChangesAsync();
                return;
            }

            throw new KeyNotFoundException("Transaction not found");
        }
    }
}
