using Libs.Data;
using Libs.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Repositories
{
    interface IUserRepository : IRepository<User>
    {

    }
    public class UserRepository : RebositoryBase<User>, IUserRepository
    {
        public UserRepository(ApplicationDBContext dBContext) : base(dBContext) { }

        void IRepository<User>.Add(User user)
        {
            _dbContext.User.Add(user);
        }

        int IRepository<User>.Count(Expression<Func<User, bool>> where)
        {
            throw new NotImplementedException();
        }

        void IRepository<User>.Delete(User entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<User>.Delete(Expression<Func<User, bool>> where)
        {
            throw new NotImplementedException();
        }

        IEnumerable<User> IRepository<User>.GetAll(Expression<Func<User, bool>> filter, Func<IQueryable<User>, IOrderedQueryable<User>> oderBy, int skip, int take)
        {
            throw new NotImplementedException();
        }

        User IRepository<User>.GetById(object id)
        {
            throw new NotImplementedException();
        }

        void IRepository<User>.SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        void IRepository<User>.Update(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
