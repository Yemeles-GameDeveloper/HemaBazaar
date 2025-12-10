using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Application.ValidationRules;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extentions
{
    public static class DependencyResolver
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            //services.AddValidatorsFromAssembly(typeof(ItemDTOValidator).Assembly);

            //services.AddValidatorsFromAssemblyContaining<CartValidator>();
            //services.AddValidatorsFromAssemblyContaining<CategoryValidator>();
            //services.AddValidatorsFromAssemblyContaining<CustomOrderValidator>();
            //services.AddValidatorsFromAssemblyContaining<FavouriteValidator>();
            //services.AddValidatorsFromAssemblyContaining<ItemListValidator>();
            //services.AddValidatorsFromAssemblyContaining<ItemValidator>();
            //services.AddValidatorsFromAssemblyContaining<OrderDetailValidator>();
            //services.AddValidatorsFromAssemblyContaining<OrderValidator>();
            //services.AddValidatorsFromAssemblyContaining<PaymentValidator>();
            //services.AddValidatorsFromAssemblyContaining<PurchaseValidator>();
            //services.AddValidatorsFromAssemblyContaining<RegisterViewModelValidator>();
            //services.AddScoped<IValidator<IEnumerable<ItemDTO>>,ItemListValidator>();


            services.AddScoped<IValidator<CartDTO>, CartDTOValidator>();
            services.AddScoped<IValidator<CategoryDTO>, CategoryDTOValidator>();
            services.AddScoped<IValidator<CustomOrderDTO>, CustomOrderDTOValidator>();
            services.AddScoped<IValidator<FavouriteDTO>, FavouriteDTOValidator>();
            services.AddScoped<IValidator<ItemDTO>, ItemDTOValidator>();
            services.AddScoped<IValidator<OrderDTO>, OrderDTOValidator>();
            services.AddScoped<IValidator<OrderDetailDTO>, OrderDetailDTOValidatior>();
            services.AddScoped<IValidator<PaymentDTO>, PaymentDTOValidator>();


            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICustomOrderService, CustomOrderService>();
            services.AddScoped<IFavouriteService, FavouriteService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPurchaseService, PurchaseService>();

        


            return services;
        }

    }
}
