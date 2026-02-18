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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    internal class ItemService : IItemService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;
        IAuditLogService _auditLogService;
        IValidator<ItemDTO> _validator;

        public ItemService(IUnitOfWork unitOfWork, IMapper mapper, IAuditLogService auditLogService, IValidator<ItemDTO> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _validator = validator;
        }

        public async Task<Result<ItemDTO>> AddAsync(ItemDTO entity)
        {
            try
            {
               FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                if (!result.IsValid)
                {
                   string errorMessages = string.Join(',',result.Errors.Select(x => x.ErrorMessage));
                    throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                }

                Item item = _mapper.Map<Item>(entity);
                item.CreatedDate = DateTime.Now;
                item.IsActive = true;
                
                await _unitOfWork.Items.AddAsync(item);
                await _unitOfWork.CompleteAsync();
                await _auditLogService.AddAsync(new AuditLog { RecordId = item.Id.ToString(), TableName = "Items", Type = LogType.Insert });
                return Result<ItemDTO>.Ok(entity, "Item created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog {  TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<ItemDTO>.Failure("Item creation failed.");
            }
        }

        public async Task<Result<IEnumerable<ItemDTO>>> AddRangeAsync(IEnumerable<ItemDTO> entities)
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

                IEnumerable<Item> items = _mapper.Map<IEnumerable<Item>>(entities);

                await _unitOfWork.Items.AddRangeAsync(items);
                await _unitOfWork.CompleteAsync();

                //? koyulamıyor hocaya sor
                await _auditLogService.AddAsync(new AuditLog {TableName = "Items", Type = LogType.Insert });

                return Result<IEnumerable<ItemDTO>>.Ok(entities, "Item created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog {  TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<ItemDTO>>.Failure("Item creation failed.");
            }
        }

        public async Task<Result<IEnumerable<ItemDTO>>> FindAsync(Expression<Func<Item, bool>> filter, OrderType orderType = OrderType.ASC, bool tracking = true, params string[] includes)
        {
            try
            {


                IEnumerable<ItemDTO> items = _mapper.Map<IEnumerable<ItemDTO>>(await _unitOfWork.Items.FindAsync(filter, orderType,tracking, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Warning });

                return Result<IEnumerable<ItemDTO>>.Ok(items, "Item got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<ItemDTO>>.Failure("Item got failed.");
            }
        }

        public async Task<Result<IEnumerable<ItemDTO>>> GetAllAsync(Expression<Func<Item, bool>> filter = null, OrderType orderType = OrderType.ASC, bool tracking = true, params string[] includes)
        {
            try
            {


                IEnumerable<ItemDTO> items = _mapper.Map<IEnumerable<ItemDTO>>(await _unitOfWork.Items.GetAllAsync(filter, orderType, tracking, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Warning });

                return Result<IEnumerable<ItemDTO>>.Ok(items, "Item got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<ItemDTO>>.Failure("Item got failed.");
            }
        }

        public async Task<Result<ItemDTO?>> GetByIdAsync(int id, bool tracking = true)
        {
            try
            {


                ItemDTO item = _mapper.Map<ItemDTO>(await _unitOfWork.Items.GetByIdAsync(id,tracking));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Warning });

                return Result<ItemDTO?>.Ok(item, "Item got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<ItemDTO?>.Failure("Item got failed.");
            }
        }

        public async Task<Result<ItemDTO>> Remove(ItemDTO entity)
        {
            try
            {
                // Hard delete: Remove from database
                Item item = await _unitOfWork.Items.GetByIdAsync(entity.Id);
                if (item == null)
                {
                    return Result<ItemDTO>.Failure("Item not found.");
                }
                
                _unitOfWork.Items.Remove(item);
                await _unitOfWork.CompleteAsync();

                await _auditLogService.AddAsync(new AuditLog { RecordId = item.Id.ToString(), TableName = "Items", Type = LogType.Delete });

                return Result<ItemDTO>.Ok(entity, "Item deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<ItemDTO>.Failure("Item cannot deleted.");
            }
        }

        public async Task<Result<IEnumerable<ItemDTO>>> RemoveRange(IEnumerable<ItemDTO> entities)
        {
            try
            {


                IEnumerable<Item> items = _mapper.Map<IEnumerable<Item>>(entities);

                _unitOfWork.Items.RemoveRange(items);
                await _unitOfWork.CompleteAsync();

                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Delete });

                return Result<IEnumerable<ItemDTO>>.Ok(entities, "Items deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<ItemDTO>>.Failure("Items could not be deleted.");
            }
        }

        public async Task<Result<ItemDTO>> Update(ItemDTO entity)
        {
            try
            {
                FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                if (!result.IsValid)
                {
                    string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                    throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                }

                // Get the existing item from database
                Item existingItem = await _unitOfWork.Items.GetByIdAsync(entity.Id);
                if (existingItem == null)
                {
                    return Result<ItemDTO>.Failure("Item not found.");
                }

                // Update only the properties that should be changed
                existingItem.Title = entity.Title;
                existingItem.Description = entity.Description;
                existingItem.Content = entity.Content;
                existingItem.Price = entity.Price;
                existingItem.CategoryId = entity.CategoryId;
                existingItem.UpdatedDate = DateTime.Now;

                _unitOfWork.Items.Update(existingItem);
                await _unitOfWork.CompleteAsync();

                await _auditLogService.AddAsync(new AuditLog { RecordId = existingItem.Id.ToString(), TableName = "Items", Type = LogType.Update });

                return Result<ItemDTO>.Ok(entity, "Item updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<ItemDTO>.Failure("Item could not be updated.");
            }
        }

        public async Task<Result<IEnumerable<ItemDTO>>> UpdateRange(IEnumerable<ItemDTO> entities)
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

                IEnumerable<Item> items = _mapper.Map<IEnumerable<Item>>(entities);

                _unitOfWork.Items.UpdateRange(items);
                await _unitOfWork.CompleteAsync();

                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Update });

                return Result<IEnumerable<ItemDTO>>.Ok(entities, "Items updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Items", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<ItemDTO>>.Failure("Items could not be updated.");
            }
        }
    }
}

