using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
        
    {
        IRepository<Cart> Carts { get; }
        IRepository<Category> Categories { get; }
        IRepository<CustomOrder> CustomOrders { get; }
        IRepository<Favourite> Favourites { get; }
        IRepository<Item> Items { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }
        IRepository<Payment> Payments { get; }
        IRepository<Purchase> Purchases { get; }
        IRepository<AuditLog> AuditLogs { get; }



        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task<int> CommitTransactionAsync();
        Task RollbackTransactionAsync();






    }
}
