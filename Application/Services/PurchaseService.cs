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
    internal class PurchaseService : IPurchaseService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;
        IAuditLogService _auditLogService;

        public PurchaseService(IUnitOfWork unitOfWork, IMapper mapper, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<Result<PurchaseDTO>> AddAsync(PurchaseDTO entity)
        {
            try
            {
                Purchase purchase = _mapper.Map<Purchase>(entity);
                await _unitOfWork.Purchases.AddAsync(purchase);
                await _unitOfWork.CompleteAsync();
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entity.AppUserId, RecordId = purchase.Id.ToString(), TableName = "Purchases", Type = LogType.Insert });
                return Result<PurchaseDTO>.Ok(entity, "Purchase created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entity.AppUserId, TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<PurchaseDTO>.Failure("Purchase creation failed.");
            }
        }

        public async Task<Result<IEnumerable<PurchaseDTO>>> AddRangeAsync(IEnumerable<PurchaseDTO> entities)
        {
            try
            {
                IEnumerable<Purchase> purchases = _mapper.Map<IEnumerable<Purchase>>(entities);

                await _unitOfWork.Purchases.AddRangeAsync(purchases);
                await _unitOfWork.CompleteAsync();

                //? koyulamıyor hocaya sor
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entities.FirstOrDefault().AppUserId, TableName = "Purchases", Type = LogType.Insert });

                return Result<IEnumerable<PurchaseDTO>>.Ok(entities, "Purchase created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entities.FirstOrDefault().AppUserId, TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PurchaseDTO>>.Failure("Purchase creation failed.");
            }
        }

        public async Task<Result<IEnumerable<PurchaseDTO>>> FindAsync(Expression<Func<Purchase, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<PurchaseDTO> purchases = _mapper.Map<IEnumerable<PurchaseDTO>>(await _unitOfWork.Purchases.FindAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Warning });

                return Result<IEnumerable<PurchaseDTO>>.Ok(purchases, "Purchase got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PurchaseDTO>>.Failure("Purchase got failed.");
            }
        }

        public async Task<Result<IEnumerable<PurchaseDTO>>> GetAllAsync(Expression<Func<Purchase, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<PurchaseDTO> purchases = _mapper.Map<IEnumerable<PurchaseDTO>>(await _unitOfWork.Purchases.GetAllAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Warning });

                return Result<IEnumerable<PurchaseDTO>>.Ok(purchases, "Purchase got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PurchaseDTO>>.Failure("Purchase got failed.");
            }
        }

        public async Task<Result<PurchaseDTO?>> GetByIdAsync(int id)
        {
            try
            {


                PurchaseDTO purchase = _mapper.Map<PurchaseDTO>(await _unitOfWork.Purchases.GetByIdAsync(id));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Warning });

                return Result<PurchaseDTO?>.Ok(purchase, "Purchase got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<PurchaseDTO?>.Failure("Purchase got failed.");
            }
        }

        public async Task<Result<PurchaseDTO>> Remove(PurchaseDTO entity)
        {
            try
            {


                Purchase purchase = _mapper.Map<Purchase>(entity);

                _unitOfWork.Purchases.Remove(purchase);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Delete });

                return Result<PurchaseDTO>.Ok(entity, "Purchase deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<PurchaseDTO>.Failure("Purchase cannot deleted.");
            }
        }

        public async Task<Result<IEnumerable<PurchaseDTO>>> RemoveRange(IEnumerable<PurchaseDTO> entities)
        {
            try
            {


                IEnumerable<Purchase> purchases = _mapper.Map<IEnumerable<Purchase>>(entities);

                _unitOfWork.Purchases.RemoveRange(purchases);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Delete });

                return Result<IEnumerable<PurchaseDTO>>.Ok(entities, "Purchases deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PurchaseDTO>>.Failure("Purchases could not be deleted.");
            }
        }

        public async Task<Result<PurchaseDTO>> Update(PurchaseDTO entity)
        {
            try
            {


                Purchase purchase = _mapper.Map<Purchase>(entity);

                _unitOfWork.Purchases.Update(purchase);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Update });

                return Result<PurchaseDTO>.Ok(entity, "Purchase updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<PurchaseDTO>.Failure("Purchase could not be updated.");
            }
        }

        public async Task<Result<IEnumerable<PurchaseDTO>>> UpdateRange(IEnumerable<PurchaseDTO> entities)
        {
            try
            {


                IEnumerable<Purchase> purchases = _mapper.Map<IEnumerable<Purchase>>(entities);

                _unitOfWork.Purchases.UpdateRange(purchases);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Update });

                return Result<IEnumerable<PurchaseDTO>>.Ok(entities, "Purchases updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Purchases", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<PurchaseDTO>>.Failure("Purchases could not be deleted.");
            }
        }
    }
}

