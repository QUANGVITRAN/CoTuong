using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Data
{
    public interface IRepository<T> where T: class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T,bool>>where);
        int Count(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> oderBy = null, 
            int skip = 0,
            int take = 0
            );
        T GetById(object id);
        void SaveChanges();
    }

}
