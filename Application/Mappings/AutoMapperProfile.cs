using Application.DTOs;
using Application.ViewModels;
using AutoMapper;
using Domain.Entities;
using HemaBazaar.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cart,CartDTO>()
                .ForMember(x=>x.ItemTitle, opt=>opt.MapFrom(x=>x.Item.Title))
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.AppUser.UserName))
                .ReverseMap();

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
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.AppUser.UserName))
                .ForMember(x => x.ItemTitle, opt => opt.MapFrom(x => x.Item.Title))
                .ReverseMap();


            CreateMap<AppUser, RegisterViewModel>().ReverseMap();
            CreateMap<AppUser, LoginViewModel>().ReverseMap();
            CreateMap<AppUser, ProfileUpdateViewModel>().ReverseMap();
        }
    }
}
