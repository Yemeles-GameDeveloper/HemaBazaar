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
    public class UnitOfWork : IUnitOfWork
    {



        readonly HemaBazaarDBContext dbContext;
        readonly HemaBazaarLogDBContext logContext;
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

       
        public UnitOfWork(HemaBazaarDBContext dbContext, HemaBazaarLogDBContext logDBContext)
        {
            this.dbContext = dbContext;
            this.logContext = logDBContext;
        }

        public IRepository<Cart> Carts => carts ??= new Repository<Cart>(dbContext);

        public IRepository<Category> Categories => categories ??= new Repository<Category>(dbContext);

        public IRepository<CustomOrder> CustomOrders => customOrders ??= new Repository<CustomOrder>(dbContext);

        public IRepository<Favourite> Favourites => favourites ??= new Repository<Favourite>(dbContext);

        public IRepository<Item> Items => items ??= new Repository<Item>(dbContext);

        public IRepository<Order> Orders => orders ??= new Repository<Order>(dbContext);

        public IRepository<OrderDetail> OrderDetails => orderDetails ??= new Repository<OrderDetail>(dbContext);

        public IRepository<Payment> Payments => payments ??= new Repository<Payment>(dbContext);

        public IRepository<Purchase> Purchases => purchases ??= new Repository<Purchase>(dbContext);

        public IRepository<AuditLog> AuditLogs => auditLogs ??= new Repository<AuditLog>(logContext);




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
            logContext.Dispose();
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
