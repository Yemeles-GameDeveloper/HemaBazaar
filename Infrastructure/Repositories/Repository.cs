using Domain;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{
    public class Repository<TEntity,TDbContext> : IRepository<TEntity>
        where TEntity : class, IBaseEntity, new()
        where TDbContext : DbContext
    {
        readonly TDbContext dBContext;
        readonly DbSet<TEntity> dbSet;

        public Repository(TDbContext dbContext)
        {
            this.dBContext = dbContext;
            dbSet = dBContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {

            IQueryable<TEntity> query = dbSet.AsQueryable();
            query = query.Where(filter);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            query = orderType == OrderType.ASC ?  query.OrderBy(e => e.Id) : query.OrderByDescending(x => x.Id);
            return await query.ToListAsync();
        }

        

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            IQueryable<TEntity> query = dbSet.AsQueryable();

            if (filter!=null)
                query = query.Where(filter);

            if(includes!=null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            query = orderType == OrderType.ASC ?  query.OrderBy(e => e.Id) : query.OrderByDescending(x => x.Id);
            return await query.ToListAsync();

        }

        public async Task AddAsync(TEntity entity) => await dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities) => await dbSet.AddRangeAsync(entities);

        public async Task<TEntity?> GetByIdAsync(int id) => await dbSet.FindAsync(id);
        
        public void Remove(TEntity entity)
        {
           var result = dbSet.Find(entity.Id);
            if (result != null)
                result.IsActive = false;
            dbSet.Update(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
           var result = dbSet.Where(x=>entities.Any(e=>e.Id == x.Id)).ToList();
            foreach (var item in result)
            {
                item.IsActive = false;
                dbSet.Update(item);
            }
        }
        

        public void Update(TEntity entity)
        {
            dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach(var entity in entities)
            {
                dbSet.Update(entity);
            }
        }
        

        
    }

        
}
