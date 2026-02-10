using Application.DTOs;
using Application.ViewModels;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using HemaBazaar.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    // Custom Value Resolvers for Purchase mapping
    public class TransactionIdResolver : IValueResolver<Purchase, PurchaseDTO, string>
    {
        public string Resolve(Purchase source, PurchaseDTO destination, string destMember, ResolutionContext context)
        {
            return source.Payment?.TransactionId ?? string.Empty;
        }
    }

    public class CartQuantityResolver : IValueResolver<Purchase, PurchaseDTO, int>
    {
        public int Resolve(Purchase source, PurchaseDTO destination, int destMember, ResolutionContext context)
        {
            return source.Cart?.Quantity ?? 0;
        }
    }

    public class PaymentStatusResolver : IValueResolver<Purchase, PurchaseDTO, PaymentStatus>
    {
        public PaymentStatus Resolve(Purchase source, PurchaseDTO destination, PaymentStatus destMember, ResolutionContext context)
        {
            return source.Payment?.Status ?? PaymentStatus.Pending;
        }
    }

    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cart, CartDTO>()
                .ForMember(x => x.Title, opt => opt.MapFrom(x => x.Item.Title))
                .ForMember(x => x.Description, opt => opt.MapFrom(x => x.Item.Description))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Item.Category.CategoryName))
                .ForMember(x => x.Price, opt => opt.MapFrom(x => x.Item.Price));




            CreateMap<CartDTO, Cart>();


            CreateMap<Category, CategoryDTO>().ReverseMap();

            CreateMap<CustomOrder, CustomOrderDTO>()
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category.CategoryName))
                .ReverseMap();

            CreateMap<Favourite, FavouriteDTO>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.AppUser.UserName))
                .ForMember(x => x.ItemTitle, opt => opt.MapFrom(x => x.Item.Title))
                .ReverseMap();

            CreateMap<Item, ItemDTO>()
            .ForMember(x => x.CategoryName,
            opt => opt.MapFrom(src => src.Category != null
                ? src.Category.CategoryName
                : string.Empty)) // veya "Unknown", artık ne istersen
             .ReverseMap();


            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();

            CreateMap<Order, OrderDTO>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.AppUser.UserName))
                .ReverseMap();

            CreateMap<Payment, PaymentDTO>().ReverseMap();

            CreateMap<Purchase, PurchaseDTO>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.AppUser != null ? x.AppUser.UserName : string.Empty))
                .ForMember(x => x.ItemTitle, opt => opt.MapFrom(x => x.Item != null ? x.Item.Title : string.Empty))
                .ForMember(x => x.TransactionId, opt => opt.MapFrom(x => x.Payment != null ? x.Payment.TransactionId : string.Empty))
                .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.Cart != null ? x.Cart.Quantity : 0))
                .ForMember(x => x.Status, opt => opt.MapFrom(x => x.Payment != null ? x.Payment.Status : PaymentStatus.Pending));

            CreateMap<PurchaseDTO, Purchase>()
                .ForMember(dest => dest.Payment, opt => opt.Ignore())
                .ForMember(dest => dest.Cart, opt => opt.Ignore())
                .ForMember(dest => dest.Item, opt => opt.Ignore())
                .ForMember(dest => dest.AppUser, opt => opt.Ignore());
                

            CreateMap<AppUser, RegisterViewModel>().ReverseMap();
            CreateMap<AppUser, LoginViewModel>().ReverseMap();
            CreateMap<AppUser, ProfileUpdateViewModel>().ReverseMap();
        }
    }
}
