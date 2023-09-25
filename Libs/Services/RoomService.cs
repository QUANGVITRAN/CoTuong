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
        private UserInRoomRepository userInRoomRepository;

        public RoomService(ApplicationDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.roomRepository = new RoomRepository(dbContext);
            this.userInRoomRepository = new UserInRoomRepository(dbContext);
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
        public void insertUserInRoom(UserInRoom userInRoom)
        {
            userInRoomRepository.insertUserInRoom(userInRoom);
            Save();
        }



       /* public void getUserInRoomList(Guid roomId)
        {
            userInRoomRepository.getUserInRoomList(roomId);
           
        }*/
        public List<Room> getAll()
        {
            return roomRepository.getAll();
        }
        public List<UserInRoom> getUserInRoomList(Guid roomId)
        {
            return userInRoomRepository.getUserInRoomList(roomId);
        }
        public Room getRoomById(Guid roomId)
        {
            return roomRepository.GetById(roomId);
        }
    }
}
