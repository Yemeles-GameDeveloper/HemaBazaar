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
    internal class OrderService : IOrderService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;
        IAuditLogService _auditLogService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<Result<OrderDTO>> AddAsync(OrderDTO entity)
        {
            try
            {
                Order order = _mapper.Map<Order>(entity);
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entity.AppUserId, RecordId = order.Id.ToString(), TableName = "Orders", Type = LogType.Insert });
                return Result<OrderDTO>.Ok(entity, "Order created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entity.AppUserId, TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<OrderDTO>.Failure("Order creation failed.");
            }
        }

        public async Task<Result<IEnumerable<OrderDTO>>> AddRangeAsync(IEnumerable<OrderDTO> entities)
        {
            try
            {
                IEnumerable<Order> orders = _mapper.Map<IEnumerable<Order>>(entities);

                await _unitOfWork.Orders.AddRangeAsync(orders);
                await _unitOfWork.CompleteAsync();

                //? koyulamıyor hocaya sor
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entities.FirstOrDefault().AppUserId, TableName = "Orders", Type = LogType.Insert });

                return Result<IEnumerable<OrderDTO>>.Ok(entities, "Order created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entities.FirstOrDefault().AppUserId, TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDTO>>.Failure("Order creation failed.");
            }
        }

        public async Task<Result<IEnumerable<OrderDTO>>> FindAsync(Expression<Func<Order, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<OrderDTO> orders = _mapper.Map<IEnumerable<OrderDTO>>(await _unitOfWork.Orders.FindAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Warning });

                return Result<IEnumerable<OrderDTO>>.Ok(orders, "Order got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDTO>>.Failure("Order got failed.");
            }
        }

        public async Task<Result<IEnumerable<OrderDTO>>> GetAllAsync(Expression<Func<Order, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<OrderDTO> orders = _mapper.Map<IEnumerable<OrderDTO>>(await _unitOfWork.Orders.GetAllAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Warning });

                return Result<IEnumerable<OrderDTO>>.Ok(orders, "Order got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDTO>>.Failure("Order got failed.");
            }
        }

        public async Task<Result<OrderDTO?>> GetByIdAsync(int id)
        {
            try
            {


                OrderDTO order = _mapper.Map<OrderDTO>(await _unitOfWork.Orders.GetByIdAsync(id));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Warning });

                return Result<OrderDTO?>.Ok(order, "Order got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<OrderDTO?>.Failure("Order got failed.");
            }
        }

        public async Task<Result<OrderDTO>> Remove(OrderDTO entity)
        {
            try
            {


                Order order = _mapper.Map<Order>(entity);

                _unitOfWork.Orders.Remove(order);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Delete });

                return Result<OrderDTO>.Ok(entity, "Order deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<OrderDTO>.Failure("Order cannot deleted.");
            }
        }

        public async Task<Result<IEnumerable<OrderDTO>>> RemoveRange(IEnumerable<OrderDTO> entities)
        {
            try
            {


                IEnumerable<Order> orders = _mapper.Map<IEnumerable<Order>>(entities);

                _unitOfWork.Orders.RemoveRange(orders);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Delete });

                return Result<IEnumerable<OrderDTO>>.Ok(entities, "Orders deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDTO>>.Failure("Orders could not be deleted.");
            }
        }

        public async Task<Result<OrderDTO>> Update(OrderDTO entity)
        {
            try
            {


                Order order = _mapper.Map<Order>(entity);

                _unitOfWork.Orders.Update(order);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Update });

                return Result<OrderDTO>.Ok(entity, "Order updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<OrderDTO>.Failure("Order could not be updated.");
            }
        }

        public async Task<Result<IEnumerable<OrderDTO>>> UpdateRange(IEnumerable<OrderDTO> entities)
        {
            try
            {


                IEnumerable<Order> orders = _mapper.Map<IEnumerable<Order>>(entities);

                _unitOfWork.Orders.UpdateRange(orders);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Update });

                return Result<IEnumerable<OrderDTO>>.Ok(entities, "Orders updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Orders", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDTO>>.Failure("Orders could not be deleted.");
            }
        }
    }
}

