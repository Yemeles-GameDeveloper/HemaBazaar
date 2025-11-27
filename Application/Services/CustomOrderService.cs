using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
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
    internal class CustomOrderService : ICustomOrderService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;
        IAuditLogService _auditLogService;

        public CustomOrderService(IUnitOfWork unitOfWork, IMapper mapper, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<Result<CustomOrderDTO>> AddAsync(CustomOrderDTO entity)
        {
            try
            {
                CustomOrder customOrder = _mapper.Map<CustomOrder>(entity);
                await _unitOfWork.CustomOrders.AddAsync(customOrder);
                await _unitOfWork.CompleteAsync();
                await _auditLogService.AddAsync(new AuditLog {  RecordId = customOrder.Id.ToString(), TableName = "CustomOrders", Type = LogType.Insert });
                return Result<CustomOrderDTO>.Ok(entity, "CustomOrder created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog {  TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<CustomOrderDTO>.Failure("CustomOrder creation failed.");
            }
        }

        public async Task<Result<IEnumerable<CustomOrderDTO>>> AddRangeAsync(IEnumerable<CustomOrderDTO> entities)
        {
            try
            {
                IEnumerable<CustomOrder> customOrders = _mapper.Map<IEnumerable<CustomOrder>>(entities);

                await _unitOfWork.CustomOrders.AddRangeAsync(customOrders);
                await _unitOfWork.CompleteAsync();

                //? koyulamıyor hocaya sor
                await _auditLogService.AddAsync(new AuditLog {TableName = "CustomOrders", Type = LogType.Insert });

                return Result<IEnumerable<CustomOrderDTO>>.Ok(entities, "CustomOrder created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CustomOrderDTO>>.Failure("CustomOrder creation failed.");
            }
        }

        public async Task<Result<IEnumerable<CustomOrderDTO>>> FindAsync(Expression<Func<CustomOrder, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<CustomOrderDTO> customOrders = _mapper.Map<IEnumerable<CustomOrderDTO>>(await _unitOfWork.CustomOrders.FindAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Warning });

                return Result<IEnumerable<CustomOrderDTO>>.Ok(customOrders, "CustomOrder got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CustomOrderDTO>>.Failure("CustomOrder got failed.");
            }
        }

        public async Task<Result<IEnumerable<CustomOrderDTO>>> GetAllAsync(Expression<Func<CustomOrder, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<CustomOrderDTO> customOrders = _mapper.Map<IEnumerable<CustomOrderDTO>>(await _unitOfWork.CustomOrders.GetAllAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Warning });

                return Result<IEnumerable<CustomOrderDTO>>.Ok(customOrders, "CustomOrder got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CustomOrderDTO>>.Failure("CustomOrder got failed.");
            }
        }

        public async Task<Result<CustomOrderDTO?>> GetByIdAsync(int id)
        {
            try
            {


                CustomOrderDTO customOrder = _mapper.Map<CustomOrderDTO>(await _unitOfWork.CustomOrders.GetByIdAsync(id));



                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Warning });

                return Result<CustomOrderDTO?>.Ok(customOrder, "CustomOrder got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<CustomOrderDTO?>.Failure("CustomOrder got failed.");
            }
        }

        public async Task<Result<CustomOrderDTO>> Remove(CustomOrderDTO entity)
        {
            try
            {


                CustomOrder customOrder = _mapper.Map<CustomOrder>(entity);

                _unitOfWork.CustomOrders.Remove(customOrder);

                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Delete });

                return Result<CustomOrderDTO>.Ok(entity, "CustomOrder deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<CustomOrderDTO>.Failure("CustomOrder cannot deleted.");
            }
        }

        public async Task<Result<IEnumerable<CustomOrderDTO>>> RemoveRange(IEnumerable<CustomOrderDTO> entities)
        {
            try
            {


                IEnumerable<CustomOrder> customOrders = _mapper.Map<IEnumerable<CustomOrder>>(entities);

                _unitOfWork.CustomOrders.RemoveRange(customOrders);

                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Delete });

                return Result<IEnumerable<CustomOrderDTO>>.Ok(entities, "CustomOrders deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CustomOrderDTO>>.Failure("CustomOrders could not be deleted.");
            }
        }

        public async Task<Result<CustomOrderDTO>> Update(CustomOrderDTO entity)
        {
            try
            {


                CustomOrder customOrder = _mapper.Map<CustomOrder>(entity);

                _unitOfWork.CustomOrders.Update(customOrder);

                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Update });

                return Result<CustomOrderDTO>.Ok(entity, "CustomOrder updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<CustomOrderDTO>.Failure("CustomOrder could not be updated.");
            }
        }

        public async Task<Result<IEnumerable<CustomOrderDTO>>> UpdateRange(IEnumerable<CustomOrderDTO> entities)
        {
            try
            {


                IEnumerable<CustomOrder> customOrders = _mapper.Map<IEnumerable<CustomOrder>>(entities);

                _unitOfWork.CustomOrders.UpdateRange(customOrders);

                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Update });

                return Result<IEnumerable<CustomOrderDTO>>.Ok(entities, "CustomOrders updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "CustomOrders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CustomOrderDTO>>.Failure("CustomOrders could not be deleted.");
            }
        }
    }
}

