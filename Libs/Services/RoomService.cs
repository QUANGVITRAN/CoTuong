using Libs.Entity;
using Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Services
{
    public class RoomService
    {
        private ApplicationDBContext dbContext;
        private RoomRepository roomRepository;

        public RoomService(ApplicationDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.roomRepository = new RoomRepository(dbContext);
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }
        public void insertRoom(Room room)
        {
            roomRepository.InsertRoom(room);
            Save();
        }
        public List<Room> getAll()
        {
            return roomRepository.getAll();
        }

        public Room getRoomById(Guid roomId)
        {
            return roomRepository.GetById(roomId);
        }

        public void DeleteRoom(Guid roomId)
        {
            var roomToDelete = getRoomById(roomId);
            if (roomToDelete != null)
            {
                roomRepository.DeleteRoom(roomToDelete);
                Save();
            }
        }

        public List<Room> SearchByName(string roomName)
        {
            return roomRepository.SearchByName(roomName);
        }
    }
}
