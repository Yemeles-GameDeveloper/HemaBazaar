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
    internal class CategoryService : ICategoryService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;
        IAuditLogService _auditLogService;
        IValidator<CategoryDTO> _validator;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IAuditLogService auditLogService, IValidator<CategoryDTO> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _validator = validator;
        }

        public async Task<Result<CategoryDTO>> AddAsync(CategoryDTO entity)
        {
            try
            {
                FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                if (!result.IsValid)
                {
                    string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                    throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                }

                Category category = _mapper.Map<Category>(entity);
                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.CompleteAsync();
                await _auditLogService.AddAsync(new AuditLog {  RecordId = category.Id.ToString(), TableName = "Categories", Type = LogType.Insert });
                return Result<CategoryDTO>.Ok(entity, "Category created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog {  TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<CategoryDTO>.Failure("Category creation failed.");
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> AddRangeAsync(IEnumerable<CategoryDTO> entities)
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

                IEnumerable<Category> categories = _mapper.Map<IEnumerable<Category>>(entities);

                await _unitOfWork.Categories.AddRangeAsync(categories);
                await _unitOfWork.CompleteAsync();

                //? koyulamıyor hocaya sor
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Insert });

                return Result<IEnumerable<CategoryDTO>>.Ok(entities, "Category created successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CategoryDTO>>.Failure("Category creation failed.");
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> FindAsync(Expression<Func<Category, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<CategoryDTO> categories = _mapper.Map<IEnumerable<CategoryDTO>>(await _unitOfWork.Categories.FindAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Warning });

                return Result<IEnumerable<CategoryDTO>>.Ok(categories, "Category got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CategoryDTO>>.Failure("Category got failed.");
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> GetAllAsync(Expression<Func<Category, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            try
            {


                IEnumerable<CategoryDTO> categories = _mapper.Map<IEnumerable<CategoryDTO>>(await _unitOfWork.Categories.GetAllAsync(filter, orderType, includes));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Warning });

                return Result<IEnumerable<CategoryDTO>>.Ok(categories, "Category got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CategoryDTO>>.Failure("Category got failed.");
            }
        }

        public async Task<Result<CategoryDTO?>> GetByIdAsync(int id)
        {
            try
            {


                CategoryDTO category = _mapper.Map<CategoryDTO>(await _unitOfWork.Categories.GetByIdAsync(id));



                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Warning });

                return Result<CategoryDTO?>.Ok(category, "Category got successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<CategoryDTO?>.Failure("Category got failed.");
            }
        }

        public async Task<Result<CategoryDTO>> Remove(CategoryDTO entity)
        {
            try
            {


                Category category = _mapper.Map<Category>(entity);

                _unitOfWork.Categories.Remove(category);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Delete });

                return Result<CategoryDTO>.Ok(entity, "Category deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<CategoryDTO>.Failure("Category cannot deleted.");
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> RemoveRange(IEnumerable<CategoryDTO> entities)
        {
            try
            {


                IEnumerable<Category> categories = _mapper.Map<IEnumerable<Category>>(entities);

                _unitOfWork.Categories.RemoveRange(categories);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Delete });

                return Result<IEnumerable<CategoryDTO>>.Ok(entities, "Categories deleted successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CategoryDTO>>.Failure("Categories could not be deleted.");
            }
        }

        public async Task<Result<CategoryDTO>> Update(CategoryDTO entity)
        {
            try
            {
                FluentValidation.Results.ValidationResult result = await _validator.ValidateAsync(entity);
                if (!result.IsValid)
                {
                    string errorMessages = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                    throw new ApplicationException($"Validasyon Hatası: {errorMessages}");
                }

                Category category = _mapper.Map<Category>(entity);

                _unitOfWork.Categories.Update(category);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Update });

                return Result<CategoryDTO>.Ok(entity, "Category updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<CategoryDTO>.Failure("Category could not be updated.");
            }
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> UpdateRange(IEnumerable<CategoryDTO> entities)
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

                IEnumerable<Category> categories = _mapper.Map<IEnumerable<Category>>(entities);

                _unitOfWork.Categories.UpdateRange(categories);

                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Update });

                return Result<IEnumerable<CategoryDTO>>.Ok(entities, "Categories updated successfully.");

            }
            catch (Exception e)
            {
                await _auditLogService.AddAsync(new AuditLog { TableName = "Categories", Type = LogType.Error, Action = e.Message });
                return Result<IEnumerable<CategoryDTO>>.Failure("Categories could not be deleted.");
            }
        }
    }
}

