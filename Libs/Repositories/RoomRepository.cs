﻿using Libs.Data;
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
    interface IRoomRepository: IRepository<Room> {
        public void InsertRoom(Room room)
        {
        }
       
    }

    public class RoomRepository: RepositoryBase<Room>, IRoomRepository
    {
        public RoomRepository(ApplicationDBContext dBContext): base(dBContext) { }

        public void InsertRoom(Room room)
        {
            _dbContext.Room.Add(room);
        }
        

        public List<Room> getAll()
        {
            return _dbContext.Room.ToList();
        }

       


        public IEnumerable<Room> GetAll(Expression<Func<Room, bool>> filter = null, Func<IQueryable<Room>, IOrderedQueryable<Room>> oderBy = null, int skip = 0, int take = 0)
        {
            throw new NotImplementedException();
        }


        public Room GetById(object id)
        {
            Room room = _dbContext.Room.Find(id);
            if (room != null)
                return room;
            throw new Exception("RoomId not found in Room!");
        }

        public void DeleteRoom(Room room)
        {
            _dbContext.Room.Remove(room);
        }
        public List<Room> SearchByName(string roomName)
        {
            return _dbContext.Room.Where(r => r.RoomName.Contains(roomName)).ToList();
        }
    }
}
