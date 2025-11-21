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
    public class Repository<T> : IRepository<T>
        where T : class, IBaseEntity, new()
    {
        readonly HemaBazaarDBContext dBContext;
        readonly DbSet<T> dbSet;

        public Repository(HemaBazaarDBContext hemaBazaarDBContext)
        {
            dBContext = hemaBazaarDBContext;
            dbSet = dBContext.Set<T>();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, OrderType orderType = OrderType.ASC, params string[] includes)
        {

            IQueryable<T> query = dbSet.AsQueryable();
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

        

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, OrderType orderType = OrderType.ASC, params string[] includes)
        {
            IQueryable<T> query = dbSet.AsQueryable();

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

        public async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await dbSet.AddRangeAsync(entities);

        public async Task<T> GetByIdAsync(int id) => await dbSet.FindAsync(id);
        
        public void Remove(T entity)
        {
           var result = dbSet.Find(entity.Id);
            if (result != null)
                result.IsActive = false;
            dbSet.Update(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
           var result = dbSet.Where(x=>entities.Any(e=>e.Id == x.Id)).ToList();
            foreach (var item in result)
            {
                item.IsActive = false;
                dbSet.Update(item);
            }
        }
        

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            foreach(var entity in entities)
            {
                dbSet.Update(entity);
            }
        }
        

        
    }

        
}
