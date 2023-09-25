using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CoTuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private RoomService roomService;
        private UserInRoomService userInRoomService;
        private IMemoryCache memoryCache;
        private CacheManege.CacheManage cacheManage;
        private readonly UserManager<IdentityUser> _userManager;
        public RoomController (RoomService roomService, UserInRoomService userInRoomService, 
            IMemoryCache memoryCache, UserManager<IdentityUser> userManager)
        {
            this.roomService = roomService;
            this.userInRoomService = userInRoomService;
            this.memoryCache = memoryCache;
            this.cacheManage = new CacheManege.CacheManage(memoryCache, userInRoomService, roomService);
            this._userManager = userManager;
        }
       // [Authorize]
        [HttpPost]
        [Route("insertRoom")]
        public IActionResult insertRoom(string roomName)
        {
            Room room = new Room();
            room.RoomName = roomName;
            room.Id = Guid.NewGuid();
            roomService.insertRoom(room);
            return Ok(new {status=true, message=room.RoomName});
        }

        [HttpGet]
        [Route("getAllRoom")]
        public IActionResult getAllRoom()
        {
            List<Room> roomList = new List<Room>();
            roomList = roomService.getAll();
            return Ok(new { status = true, message = roomList });
        }
        [HttpGet]
        [Route("getAllUserInRoom")]
        public IActionResult getAllUserInRoom(Guid roomId)
        {
            //List<IdentityUser> userinroomList = userInRoomService.getUserInRoom(roomId);
            List<IdentityUser> userinroomList = cacheManage.userInRoom[roomId.ToString()];
            return Ok(new { status = true, message = userinroomList });
        }
        [HttpGet]
        [Route("getRoomById")]
        public IActionResult getRoomById(Guid roomId)
        {
            Room room = roomService.getRoomById(roomId);
            return Ok(new { status = true, message = room });
        }

        [HttpPost]
        [Route("addUserToRoom")]
        public IActionResult addUserToRoom(Guid roomId, string userId)
        {
            userInRoomService.insertUserInRoom(roomId, userId);
            List<IdentityUser> userinroomList = new List<IdentityUser>();
            if (!cacheManage.userInRoom.ContainsKey(roomId.ToString().ToLower()))
            {
                userinroomList = userInRoomService.getUserInRoom(roomId);
                cacheManage.userInRoom.Add(roomId.ToString(), userinroomList);
            }
            else
            {
                userinroomList = cacheManage.userInRoom[roomId.ToString()];
                userinroomList.Add(_userManager.FindByIdAsync(userId).Result);
            }
            return Ok(new { status = true, message = "" });
        }
    }
}
