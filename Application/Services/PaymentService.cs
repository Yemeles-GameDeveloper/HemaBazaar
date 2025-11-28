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
    internal class PaymentService : IPaymentService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;
        IAuditLogService _auditLogService;
        IValidator<PaymentDTO> _validator;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IAuditLogService auditLogService, IValidator<PaymentDTO> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _validator = validator;
        }

        public async Task<Result<PaymentDTO>> AddAsync(PaymentDTO entity)
        {
            try
            {
                FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                if (!result.IsValid)
                {
                    string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                    throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                }

                Payment payment = _mapper.Map<Payment>(entity);
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.CompleteAsync();
                await _auditLogService.AddAsync(new AuditLog {  RecordId = payment.Id.ToString(), TableName = "Payments", Type = LogType.Insert });
                return Result<PaymentDTO>.Ok(entity, "Payment created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog {  TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<PaymentDTO>.Failure("Payment creation failed.");
            }
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> AddRangeAsync(IEnumerable<PaymentDTO> entities)
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

                IEnumerable<Payment> payments = _mapper.Map<IEnumerable<Payment>>(entities);

                await _unitOfWork.Payments.AddRangeAsync(payments);
                await _unitOfWork.CompleteAsync();

                //? koyulamıyor hocaya sor
                await _auditLogService.AddAsync(new AuditLog {  TableName = "Payments", Type = LogType.Insert });

                return Result<IEnumerable<PaymentDTO>>.Ok(entities, "Payment created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PaymentDTO>>.Failure("Payment creation failed.");
            }
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> FindAsync(Expression<Func<Payment, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<PaymentDTO> payments = _mapper.Map<IEnumerable<PaymentDTO>>(await _unitOfWork.Payments.FindAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Warning });

                return Result<IEnumerable<PaymentDTO>>.Ok(payments, "Payment got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PaymentDTO>>.Failure("Payment got failed.");
            }
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> GetAllAsync(Expression<Func<Payment, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<PaymentDTO> payments = _mapper.Map<IEnumerable<PaymentDTO>>(await _unitOfWork.Payments.GetAllAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Warning });

                return Result<IEnumerable<PaymentDTO>>.Ok(payments, "Payment got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PaymentDTO>>.Failure("Payment got failed.");
            }
        }

        public async Task<Result<PaymentDTO?>> GetByIdAsync(int id)
        {
            try
            {


                PaymentDTO payment = _mapper.Map<PaymentDTO>(await _unitOfWork.Payments.GetByIdAsync(id));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Warning });

                return Result<PaymentDTO?>.Ok(payment, "Payment got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<PaymentDTO?>.Failure("Payment got failed.");
            }
        }

        public async Task<Result<PaymentDTO>> Remove(PaymentDTO entity)
        {
            try
            {


                Payment payment = _mapper.Map<Payment>(entity);

                _unitOfWork.Payments.Remove(payment);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Delete });

                return Result<PaymentDTO>.Ok(entity, "Payment deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<PaymentDTO>.Failure("Payment cannot deleted.");
            }
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> RemoveRange(IEnumerable<PaymentDTO> entities)
        {
            try
            {


                IEnumerable<Payment> payments = _mapper.Map<IEnumerable<Payment>>(entities);

                _unitOfWork.Payments.RemoveRange(payments);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Delete });

                return Result<IEnumerable<PaymentDTO>>.Ok(entities, "Payments deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PaymentDTO>>.Failure("Payments could not be deleted.");
            }
        }

        public async Task<Result<PaymentDTO>> Update(PaymentDTO entity)
        {

            try
            {
                FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                if (!result.IsValid)
                {
                    string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                    throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                }

                Payment payment = _mapper.Map<Payment>(entity);

                _unitOfWork.Payments.Update(payment);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Update });

                return Result<PaymentDTO>.Ok(entity, "Payment updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<PaymentDTO>.Failure("Payment could not be updated.");
            }
        }


        
        public async Task<Result<IEnumerable<PaymentDTO>>> UpdateRange(IEnumerable<PaymentDTO> entities)
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
                
                IEnumerable<Payment> payments = _mapper.Map<IEnumerable<Payment>>(entities);

                _unitOfWork.Payments.UpdateRange(payments);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Update });

                return Result<IEnumerable<PaymentDTO>>.Ok(entities, "Payments updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Payments", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PaymentDTO>>.Failure("Payments could not be deleted.");
            }
        }
    }
}

