using Libs.Entity;
using Libs.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Services
{
    public class UserInRoomService
    {
        private ApplicationDBContext dbContext;
        private UserInRoomRepository userInRoomRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public UserInRoomService(ApplicationDBContext dbContext, UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            userInRoomRepository = new UserInRoomRepository(dbContext);
            this._userManager = userManager;
        }

        public void insertUserInRoom(Guid roomId,String userId )
        {
            UserInRoom userInRoom = new UserInRoom();
            userInRoom.Id =  Guid.NewGuid();
            userInRoom.UserId = userId;
            userInRoom.RoomId = roomId;
            userInRoomRepository.insertUserInRoom(userInRoom);
            userInRoomRepository.Save();

        }

        public void delUserInRoom( Guid roomId,  String userId) 
        {
            var userInRoom = dbContext.UserInRoom.Where(x => x.UserId.Equals(userId)|| x.RoomId.Equals(roomId)).FirstOrDefault();
            userInRoomRepository.delUserInRoom(userInRoom);
            userInRoomRepository.Save();
        }

        public List<IdentityUser> getUserInRoom(Guid roomId)
        {
            var listUser = new List<IdentityUser>();
            var userInRooms = userInRoomRepository.getUserInRoomList(roomId);
            foreach (var userInRoom in userInRooms)
            {
                IdentityUser identityUser = _userManager.FindByIdAsync(userInRoom.UserId).Result;
                listUser.Add(identityUser);
            }
            return listUser;
        }
    }
}
