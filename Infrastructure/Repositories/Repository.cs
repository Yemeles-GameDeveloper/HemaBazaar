using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class, IBaseEntity, new()
    {
        readonly HemaBazaarDBContext dBContext;

        public Repository(HemaBazaarDBContext hemaBazaarDBContext)
        {
            dBContext = hemaBazaarDBContext;
        }

        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
        public void Remove(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
        public Task<T> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        

        
        
    }
}
