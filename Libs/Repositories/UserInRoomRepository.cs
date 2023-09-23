using Libs.Data;
using Libs.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Repositories
{
    public interface IUserInRoomRepository: IRepository<UserInRoom>
    {
        public void insertUserInRoom(UserInRoom UserInRoom);
        public List<UserInRoom> getUserInRoomList(Guid roomId);
    }
    public class UserInRoomRepository : RebositoryBase<UserInRoom>, IUserInRoomRepository
    {
        public UserInRoomRepository(ApplicationDBContext dBContext) : base(dBContext) { }

        public IEnumerable<UserInRoom> GetAll(Expression<Func<UserInRoom, bool>> filter = null, Func<IQueryable<UserInRoom>, IOrderedQueryable<UserInRoom>> oderBy = null, int skip = 0, int take = 0)
        {
            throw new NotImplementedException();
        }

        public UserInRoom GetById(object id)
        {
            throw new NotImplementedException();
        }

        public void insertUserInRoom(UserInRoom userInRoom)
        {
            _dbContext.UserInRoom.Add(userInRoom);
        }
        public List<UserInRoom> getUserInRoomList(Guid roomid)
        {
            return _dbContext.UserInRoom.Where(s=>s.RoomId== roomid).ToList();
        }
    }
}
