using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {



        readonly DbContext dbContext;
        
        IDbContextTransaction transaction;

        public IRepository<Cart>? carts;

        public IRepository<Category>? categories;

        public IRepository<CustomOrder>? customOrders;

        public IRepository<Favourite>? favourites;

        public IRepository<Item>? items;

        public IRepository<Order>? orders;

        public IRepository<OrderDetail>? orderDetails;

        public IRepository<Payment>? payments;

        public IRepository<Purchase>? purchases;

        public IRepository<AuditLog>? auditLogs;

       
        public UnitOfWork(DbContext dbContext)
        {
            this.dbContext = dbContext;
            
        }

        public IRepository<Cart> Carts => carts ??= new Repository<Cart,DbContext>(dbContext);

        public IRepository<Category> Categories => categories ??= new Repository<Category, DbContext>(dbContext);

        public IRepository<CustomOrder> CustomOrders => customOrders ??= new Repository<CustomOrder, DbContext>(dbContext);

        public IRepository<Favourite> Favourites => favourites ??= new Repository<Favourite, DbContext>(dbContext);

        public IRepository<Item> Items => items ??= new Repository<Item, DbContext>(dbContext);

        public IRepository<Order> Orders => orders ??= new Repository<Order, DbContext>(dbContext);

        public IRepository<OrderDetail> OrderDetails => orderDetails ??= new Repository<OrderDetail, DbContext>(dbContext);

        public IRepository<Payment> Payments => payments ??= new Repository<Payment, DbContext>(dbContext);

        public IRepository<Purchase> Purchases => purchases ??= new Repository<Purchase, DbContext>(dbContext);

        public IRepository<AuditLog> AuditLogs => auditLogs ??= new Repository<AuditLog, DbContext>(dbContext);




        public async Task BeginTransactionAsync() => transaction = await dbContext.Database.BeginTransactionAsync();
        

        public async Task CommitTransactionAsync()
        {
            if (transaction == null) return;

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            await dbContext.DisposeAsync();
            transaction = null;

        }

        public async Task<int> CompleteAsync() => await dbContext.SaveChangesAsync();
        

        public void Dispose()
        {
            transaction?.Dispose();
            dbContext.Dispose();
            dbContext.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            if (transaction == null) return;
            await transaction.RollbackAsync();
            await dbContext.DisposeAsync();
            transaction = null;

        }
    }
}
