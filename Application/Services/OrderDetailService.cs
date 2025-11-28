using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    internal class OrderDetailService :IOrderDetailService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;
        IAuditLogService _auditLogService;
        IValidator<OrderDetailDTO> _validator;

        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper, IAuditLogService auditLogService, IValidator<OrderDetailDTO> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _validator = validator;
        }

        public async Task<Result<OrderDetailDTO>> AddAsync(OrderDetailDTO entity)
        {
            try
            {
                FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                if (!result.IsValid)
                {
                    string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                    throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                }

                OrderDetail orderDetail = _mapper.Map<OrderDetail>(entity);
                await _unitOfWork.OrderDetails.AddAsync(orderDetail);
                await _unitOfWork.CompleteAsync();
                await _auditLogService.AddAsync(new AuditLog {  RecordId = orderDetail.Id.ToString(), TableName = "OrderDetails", Type = LogType.Insert });
                return Result<OrderDetailDTO>.Ok(entity, "OrderDetail created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<OrderDetailDTO>.Failure("OrderDetail creation failed.");
            }
        }

        public async Task<Result<IEnumerable<OrderDetailDTO>>> AddRangeAsync(IEnumerable<OrderDetailDTO> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                    if (!result.IsValid)
                    {
                        string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                        throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                    }
                }

                IEnumerable<OrderDetail> orderDetails = _mapper.Map<IEnumerable<OrderDetail>>(entities);

                await _unitOfWork.OrderDetails.AddRangeAsync(orderDetails);
                await _unitOfWork.CompleteAsync();

                //? koyulamıyor hocaya sor
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Insert });

                return Result<IEnumerable<OrderDetailDTO>>.Ok(entities, "OrderDetail created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDetailDTO>>.Failure("OrderDetail creation failed.");
            }
        }

        public async Task<Result<IEnumerable<OrderDetailDTO>>> FindAsync(Expression<Func<OrderDetail, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<OrderDetailDTO> orderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(await _unitOfWork.OrderDetails.FindAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Warning });

                return Result<IEnumerable<OrderDetailDTO>>.Ok(orderDetails, "OrderDetail got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDetailDTO>>.Failure("OrderDetail got failed.");
            }
        }

        public async Task<Result<IEnumerable<OrderDetailDTO>>> GetAllAsync(Expression<Func<OrderDetail, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<OrderDetailDTO> orderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(await _unitOfWork.OrderDetails.GetAllAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Warning });

                return Result<IEnumerable<OrderDetailDTO>>.Ok(orderDetails, "OrderDetail got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDetailDTO>>.Failure("OrderDetail got failed.");
            }
        }

        public async Task<Result<OrderDetailDTO?>> GetByIdAsync(int id)
        {
            try
            {


                OrderDetailDTO orderDetail = _mapper.Map<OrderDetailDTO>(await _unitOfWork.OrderDetails.GetByIdAsync(id));



                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Warning });

                return Result<OrderDetailDTO?>.Ok(orderDetail, "OrderDetail got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<OrderDetailDTO?>.Failure("OrderDetail got failed.");
            }
        }

        public async Task<Result<OrderDetailDTO>> Remove(OrderDetailDTO entity)
        {
            try
            {


                OrderDetail orderDetail = _mapper.Map<OrderDetail>(entity);

                _unitOfWork.OrderDetails.Remove(orderDetail);

                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Delete });

                return Result<OrderDetailDTO>.Ok(entity, "OrderDetail deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<OrderDetailDTO>.Failure("OrderDetail cannot deleted.");
            }
        }

        public async Task<Result<IEnumerable<OrderDetailDTO>>> RemoveRange(IEnumerable<OrderDetailDTO> entities)
        {
            try
            {


                IEnumerable<OrderDetail> orderDetails = _mapper.Map<IEnumerable<OrderDetail>>(entities);

                _unitOfWork.OrderDetails.RemoveRange(orderDetails);

                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Delete });

                return Result<IEnumerable<OrderDetailDTO>>.Ok(entities, "OrderDetails deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDetailDTO>>.Failure("OrderDetails could not be deleted.");
            }
        }

        public async Task<Result<OrderDetailDTO>> Update(OrderDetailDTO entity)
        {
            try
            {
                FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                if (!result.IsValid)
                {
                    string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                    throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                }

                OrderDetail orderDetail = _mapper.Map<OrderDetail>(entity);

                _unitOfWork.OrderDetails.Update(orderDetail);

                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Update });

                return Result<OrderDetailDTO>.Ok(entity, "OrderDetail updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<OrderDetailDTO>.Failure("OrderDetail could not be updated.");
            }
        }

        public async Task<Result<IEnumerable<OrderDetailDTO>>> UpdateRange(IEnumerable<OrderDetailDTO> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                    if (!result.IsValid)
                    {
                        string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                        throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                    }
                }

                IEnumerable<OrderDetail> orderDetails = _mapper.Map<IEnumerable<OrderDetail>>(entities);

                _unitOfWork.OrderDetails.UpdateRange(orderDetails);

                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Update });

                return Result<IEnumerable<OrderDetailDTO>>.Ok(entities, "OrderDetails updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "OrderDetails", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<OrderDetailDTO>>.Failure("OrderDetails could not be deleted.");
            }
        }
    }
}

