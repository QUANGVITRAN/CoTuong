using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Data
{
    public abstract class RebositoryBase<T> where T : class
    {
        protected ApplicationDBContext _dbContext;
        protected readonly DbSet<T> dbSet;

        protected RebositoryBase(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }
        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }
        public virtual void Update(T entity)
        {
            dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            dbSet.Remove(entity);
        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbSet.Remove(obj);
            
        }
        public virtual int Count(Expression<Func<T, bool>> where)
        {
            return dbSet.Count(where);
        }
        public virtual void GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> oderBy = null,
            string includeProterties = "",
            int skip = 0,
            int take = 0)
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
                query = query.Where(filter);
            foreach(var includeProterty in includeProterties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProterty);
            }
            if(oderBy != null)
                query = oderBy(query);
            if(skip > 0)
                query = query.Skip(skip);
            if(take > 0)
                query= query.Take(take);
        }
        public virtual void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
