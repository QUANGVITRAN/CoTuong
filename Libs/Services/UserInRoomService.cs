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
            userInRoom.Id = Guid.NewGuid();
            userInRoom.UserId = userId;
            userInRoom.RoomId = roomId;

            int sl = getUserInRoom(roomId).Count();
            if(sl > 1)
            {
                userInRoom.Role = "NguoiXem";
            }
            else if (sl == 1)
            {
                userInRoom.Role = "DoiThu";
            }
            else
            {
                userInRoom.Role = "ChuPhong";
            }

            userInRoomRepository.insertUserInRoom(userInRoom);
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
        public UserInRoom getUserInRoomById(Guid roomId, string userId)
        {
            var listUser = userInRoomRepository.getUserInRoomList(roomId);
            var userInRooms = listUser.Where(u => u.UserId == userId).FirstOrDefault();

            return userInRooms;
        }
        public List<UserInRoom> getAllUserInRoom(Guid roomId)
        {
            var listUser = userInRoomRepository.getUserInRoomList(roomId);
            
            return listUser;
        }

        public void delUserInRoom(Guid roomId, String userId)
        {
            var userInRoom = dbContext.UserInRoom.Where(x => x.UserId.Equals(userId) && x.RoomId.Equals(roomId)).FirstOrDefault();
            userInRoomRepository.delUserInRoom(userInRoom);
            userInRoomRepository.Save();
        }

        public void updateUserInRoom(UserInRoom user)
        {
            userInRoomRepository.Update(user);
            userInRoomRepository.Save();
        }
    }
}
