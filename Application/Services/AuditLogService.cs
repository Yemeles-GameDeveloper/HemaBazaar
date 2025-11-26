using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    internal class AuditLogService : IAuditLogService
    {
        IUnitOfWork _unitOfWork;

        public AuditLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuditLog>> AddAsync(AuditLog entity)
        {
            try
            {
                await _unitOfWork.AuditLogs.AddAsync(entity);
                await _unitOfWork.CompleteAsync();
                return Result<AuditLog>.Ok(entity,"Log created successfully.");

            }
            catch(Exception)
            {
                return Result<AuditLog>.Failure("Log creation failed.");
            }
        }

        public async Task<Result<IEnumerable<AuditLog>>> AddRangeAsync(IEnumerable<AuditLog> entities)
        {
            try
            {
                await _unitOfWork.AuditLogs.AddRangeAsync(entities);
                await _unitOfWork.CompleteAsync();
                return Result<IEnumerable<AuditLog>>.Ok(entities, "Log created successfully.");

            }
            catch (Exception)
            {
                return Result<IEnumerable<AuditLog>>.Failure("Log creation failed.");
            }
        }

        public async Task<Result<IEnumerable<AuditLog>>> FindAsync(Expression<Func<AuditLog, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {
                IEnumerable<AuditLog> result = await _unitOfWork.AuditLogs.FindAsync(filter,orderType,includes);
                
                return Result<IEnumerable<AuditLog>>.Ok(result);

            }
            catch (Exception)
            {
                return Result<IEnumerable<AuditLog>>.Failure("Log failed.");
            }
        }

        public async Task<Result<IEnumerable<AuditLog>>> GetAllAsync(Expression<Func<AuditLog, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {
                IEnumerable<AuditLog> result = await _unitOfWork.AuditLogs.GetAllAsync(filter, orderType, includes);

                return Result<IEnumerable<AuditLog>>.Ok(result);

            }
            catch (Exception)
            {
                return Result<IEnumerable<AuditLog>>.Failure("Log failed.");
            }
        }

        public async Task<Result<AuditLog?>> GetByIdAsync(int id)
        {
            try
            {
                AuditLog result = await _unitOfWork.AuditLogs.GetByIdAsync(id);

                return Result<AuditLog>.Ok(result);

            }
            catch (Exception)
            {
                return Result<AuditLog>.Failure("Log failed.");
            }
        }
    }
}
