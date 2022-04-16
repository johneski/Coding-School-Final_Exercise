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
            await _fuelStationContext.SaveChangesAsync();
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
            var transaction = await _fuelStationContext.Transactions.AsNoTracking().Include(x => x.TransactionLines)
                                                                                    .Include(x => x.Employee)
                                                                                    .Include(x => x.Customer)
                                                                                    .SingleOrDefaultAsync(x => x.Id == id);
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
                transaction.IsActive = entity.IsActive;
                transaction.TransactionLines.Clear();

                foreach (var line in entity.TransactionLines)
                {
                    var existLine = await _fuelStationContext.TransactionLines.SingleOrDefaultAsync(x => x.Id == line.Id);
                    if (existLine is not null)
                    {
                        existLine.ItemId = line.ItemId;
                        existLine.ItemPrice = line.ItemPrice;
                        existLine.DiscountPercent = line.DiscountPercent;
                        existLine.DiscountValue = line.DiscountValue;
                        existLine.NetValue = line.NetValue;
                        existLine.Qty = line.Qty;
                        existLine.TotalValue = line.TotalValue;
                    }
                    else
                    {
                        transaction.TransactionLines.Add(new TransactionLine()
                        {
                            ItemId = line.ItemId,
                            ItemPrice = line.ItemPrice,
                            DiscountPercent = line.DiscountPercent,
                            DiscountValue = line.DiscountValue,
                            NetValue = line.NetValue,
                            Qty = line.Qty,
                            TotalValue = line.TotalValue,
                            TransactionId = line.TransactionId,
                        });
                    }
                    

                }
                var transLinesId = transaction.TransactionLines.Select(x => x.Id).ToList();
                var entityLinesId = entity.TransactionLines.Select(x => x.Id).ToList();

                foreach (var lineId in transLinesId)
                {
                    if (!entityLinesId.Contains(lineId))
                    {   
                        var lineToDelete = await _fuelStationContext.TransactionLines.SingleOrDefaultAsync(x => x.Id == lineId);
                        if (lineToDelete is null) continue;
                        _fuelStationContext.TransactionLines.Remove(lineToDelete);
                    }
                }

                _fuelStationContext.Transactions.Update(transaction);
                await _fuelStationContext.SaveChangesAsync();
                return;
            }

            throw new KeyNotFoundException("Transaction not found");
        }
    }
}
