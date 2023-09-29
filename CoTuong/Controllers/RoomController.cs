using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoomController (RoomService roomService, UserInRoomService userInRoomService, 
            IMemoryCache memoryCache, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.roomService = roomService;
            this.userInRoomService = userInRoomService;
            this.memoryCache = memoryCache;
            this.cacheManage = new CacheManege.CacheManage(memoryCache, userInRoomService, roomService);
            this._userManager = userManager;
            this._httpContextAccessor = httpContextAccessor;
        }

        //[Authorize]
        [HttpPost]
        [Route("insertRoom")]
        public IActionResult insertRoom(string roomName)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                Room room = new Room();
                room.RoomName = roomName;
                room.Id = Guid.NewGuid();
                roomService.insertRoom(room);

                // thêm user vừa tạo vào phòng
               
                string userId = User.FindFirst("Id")?.Value;

                userInRoomService.insertUserInRoom(room.Id, userId);
                List<IdentityUser> userinroomList = new List<IdentityUser>();
                if (!cacheManage.userInRoom.ContainsKey(room.Id.ToString().ToLower()))
                {
                    userinroomList = userInRoomService.getUserInRoom(room.Id);
                    cacheManage.userInRoom.Add(room.Id.ToString(), userinroomList);
                }
                else
                {
                    userinroomList = cacheManage.userInRoom[room.Id.ToString()];
                    userinroomList.Add(_userManager.FindByIdAsync(userId).Result);
                }

                return Ok(new { status = true, message = room.RoomName });
                
            }
            catch(Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }
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
            try
            {
                List<IdentityUser> userinroomList = cacheManage.userInRoom[roomId.ToString()];
                return Ok(new { status = true, message = userinroomList });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });
            }            
        }

        [HttpGet]
        [Route("getRoomById")]
        public IActionResult getRoomById(Guid roomId)
        {
            try
            {
                Room room = roomService.getRoomById(roomId);
                return Ok(new { status = true, message = room });
                
            }
            catch(Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("addUserToRoom")]
        public IActionResult addUserToRoom(Guid roomId, string userId)
        {
            try
            {
                //todo: thêm user vào room (truy cập vào DB)
                userInRoomService.insertUserInRoom(roomId, userId); // thêm user vào room

                //todo: update list userInRoom trong cache
                List<IdentityUser> userinroomList = new List<IdentityUser>(); // tạo list user mới
                if (!cacheManage.userInRoom.ContainsKey(roomId.ToString().ToLower())) // kiểm tra trong cache có list user chưa
                {                                                                       // nếu chưa
                    userinroomList = userInRoomService.getUserInRoom(roomId); // lấy list user từ DB
                    cacheManage.userInRoom.Add(roomId.ToString(), userinroomList); //thêm vòa cache
                }
                else
                {                                                                  // nếu có
                    userinroomList = cacheManage.userInRoom[roomId.ToString()]; //lấy dữ list user từ cache
                    userinroomList.Add(_userManager.FindByIdAsync(userId).Result); // thêm  user nào list trong cache
                }
                return Ok(new { status = true, message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex });
            }
        }

        [HttpDelete]
        [Route("deleteRoom")]
        public IActionResult deleteRoom(Guid roomId)
        {
            try
            {
                roomService.DeleteRoom(roomId);
                if (cacheManage.userInRoom.ContainsKey(roomId.ToString().ToLower()))
                    cacheManage.userInRoom.Remove(roomId.ToString().ToLower());
                return Ok(new { status = true, message = "Xoa Room thanh cong" });
            }
            catch(Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("SearchByName")]
        public IActionResult SearchByName(string roomName)
        {
            List<Room> roomList = roomService.SearchByName(roomName);
            return Ok(new { status = true, message = roomList });
        }
    }
}
