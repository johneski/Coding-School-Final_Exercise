using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Repositories
{
    public interface IEntityRepo<TEntity>
    {
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> GetAllActiveAsync();
        Task<List<TEntity>> GetAllInactiveAsync();
        Task<TEntity?> GetByIdAsync(Guid id, bool active);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(Guid id, TEntity entity);
        Task DeleteAsync(Guid id);        

    }
}
