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
     //17 Kasım 2:05:21 den devam et.
    internal class CartService : ICartService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;
        IAuditLogService _auditLogService;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<Result<CartDTO>> AddAsync(CartDTO entity)
        {
            try
            {
                Cart cart = _mapper.Map<Cart>(entity);
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.CompleteAsync();
                await _auditLogService.AddAsync(new AuditLog {AppUserId=entity.AppUserId,RecordId=cart.Id.ToString(),TableName="Carts",Type=LogType.Insert });
                return Result<CartDTO>.Ok(entity, "Cart created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entity.AppUserId, TableName = "Carts", Type = LogType.Error, Action= e.Message});
                return Result<CartDTO>.Failure("Cart creation failed.");
            }
        }

        public async Task<Result<IEnumerable< CartDTO>>> AddRangeAsync(IEnumerable<CartDTO> entities)
        {
            try
            {
                IEnumerable<Cart> carts = _mapper.Map<IEnumerable<Cart>>(entities);

                await _unitOfWork.Carts.AddRangeAsync(carts);
                await _unitOfWork.CompleteAsync();

                //? koyulamıyor hocaya sor
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entities.FirstOrDefault().AppUserId, TableName = "Carts", Type = LogType.Insert });

                return Result<IEnumerable< CartDTO>>.Ok(entities, "Cart created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { AppUserId = entities.FirstOrDefault().AppUserId, TableName = "Carts", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CartDTO>>.Failure("Cart creation failed.");
            }
        }

        public async Task<Result<IEnumerable<CartDTO>>> FindAsync(Expression<Func<Cart, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {
                

                IEnumerable<CartDTO> carts = _mapper.Map<IEnumerable<CartDTO>>(await _unitOfWork.Carts.FindAsync(filter,orderType,includes));
                

                
                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Warning });

                return Result<IEnumerable<CartDTO>>.Ok(carts, "Cart got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CartDTO>>.Failure("Cart got failed.");
            }
        }

        public async Task<Result<IEnumerable<CartDTO>>> GetAllAsync(Expression<Func<Cart, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<CartDTO> carts = _mapper.Map<IEnumerable<CartDTO>>(await _unitOfWork.Carts.GetAllAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Warning });

                return Result<IEnumerable<CartDTO>>.Ok(carts, "Cart got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CartDTO>>.Failure("Cart got failed.");
            }
        }

        public async Task<Result<CartDTO?>> GetByIdAsync(int id)
        {
            try
            {


                CartDTO cart = _mapper.Map<CartDTO>(await _unitOfWork.Carts.GetByIdAsync(id));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Warning });

                return Result<CartDTO?>.Ok(cart, "Cart got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Error, Action = e.Message });
                return Result<CartDTO?>.Failure("Cart got failed.");
            }
        }

        public async Task <Result<CartDTO>> Remove(CartDTO entity)
        {
            try
            {


                Cart cart = _mapper.Map<Cart>(entity);

                 _unitOfWork.Carts.Remove(cart);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Delete });

                return Result<CartDTO>.Ok(entity, "Cart deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Error, Action = e.Message });
                return Result<CartDTO>.Failure("Cart cannot deleted.");
            }
        }

        public async Task<Result<IEnumerable< CartDTO>>> RemoveRange(IEnumerable<CartDTO> entities)
        {
            try
            {


                IEnumerable<Cart> carts = _mapper.Map<IEnumerable< Cart>>(entities);

                _unitOfWork.Carts.RemoveRange(carts);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Delete });

                return Result<IEnumerable< CartDTO>>.Ok(entities, "Carts deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CartDTO>>.Failure("Carts could not be deleted.");
            }
        }

        public async Task<Result<CartDTO>> Update(CartDTO entity)
        {
            try
            {


                Cart cart = _mapper.Map<Cart>(entity);

                _unitOfWork.Carts.Update(cart);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Update });

                return Result<CartDTO>.Ok(entity, "Cart updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Error, Action = e.Message });
                return Result<CartDTO>.Failure("Cart could not be updated.");
            }
        }

        public async Task<Result<IEnumerable <CartDTO>>> UpdateRange(IEnumerable<CartDTO> entities)
        {
            try
            {


                IEnumerable<Cart> carts = _mapper.Map<IEnumerable<Cart>>(entities);

                _unitOfWork.Carts.UpdateRange(carts);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Update });

                return Result<IEnumerable<CartDTO>>.Ok(entities, "Carts updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Carts", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CartDTO>>.Failure("Carts could not be deleted.");
            }
        }
    }
}
