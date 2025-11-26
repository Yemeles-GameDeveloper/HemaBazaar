using Application.Common;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    internal interface IGenericService<T,TDTO>
        where T : class, IBaseEntity, new()
        
    {
        Task<Result<TDTO>> AddAsync(TDTO entity);
        Task< Result<IEnumerable<TDTO>>>AddRangeAsync(IEnumerable<TDTO> entities);

        Result<T> Update(TDTO entity);
        Result<T> UpdateRange(IEnumerable<TDTO> entities);
        Result<T> Remove(TDTO entity);
        Result<T> RemoveRange(IEnumerable<TDTO> entities);

        Task<Result<TDTO?>> GetByIdAsync(int id);
        Task<Result<IEnumerable<TDTO>>> GetAllAsync(Expression<Func<T, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes);
        Task<Result<IEnumerable<TDTO>>> FindAsync(Expression<Func<T, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes);

    }
}
