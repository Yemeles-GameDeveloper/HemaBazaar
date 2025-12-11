using Application.Common;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuditLogService 
    {
        Task<Result<AuditLog>> AddAsync(AuditLog entity);
        Task<Result<IEnumerable<AuditLog>>> AddRangeAsync(IEnumerable<AuditLog> entities);
        Task<Result<AuditLog?>> GetByIdAsync(int id);
        Task<Result<IEnumerable<AuditLog>>> GetAllAsync(Expression<Func<AuditLog, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes);
        Task<Result<IEnumerable<AuditLog>>> FindAsync(Expression<Func<AuditLog, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes);
    }
}
