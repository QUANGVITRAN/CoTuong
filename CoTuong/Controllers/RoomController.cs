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
using System.Collections.Generic;
using Libs.DTOs;
using Microsoft.Extensions.Hosting;

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
        private IWebHostEnvironment hostEnvironment;

        public RoomController(RoomService roomService, UserInRoomService userInRoomService, IWebHostEnvironment hostEnvironment,
            IMemoryCache memoryCache, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.roomService = roomService;
            this.userInRoomService = userInRoomService;
            this.memoryCache = memoryCache;
            this.cacheManage = new CacheManege.CacheManage(memoryCache, userInRoomService, roomService);
            this._userManager = userManager;
            this._httpContextAccessor = httpContextAccessor;
            this.hostEnvironment = hostEnvironment;
        }

        //[Authorize]
        [HttpPost]
        [Route("insertRoom")]
        public IActionResult insertRoom(string roomName, string userId)
        {
            try
            {
                string chessJson = System.IO.File.ReadAllText(hostEnvironment.ContentRootPath + "\\Data\\ChessJson.txt");
                var httpContext = _httpContextAccessor.HttpContext;
                Room room = new Room();
                room.RoomName = roomName;
                room.Id = Guid.NewGuid();
                room.Turn = 0;
                room.ChessMap = chessJson;
                roomService.insertRoom(room);

                // thêm user vừa tạo vào phòng

              //  string userId = User.FindFirst("Id")?.Value;

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

                return Ok(new { status = true, message = room.Id });

            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }
        }

        [HttpGet]
        [Route("getAllRoom")]
        public IActionResult getAllRoom()
        {
            List<Room> roomList = roomService.getAll();
            List<ResListRoom> resListRooms = new List<ResListRoom>();
             
            foreach (Room room in roomList)
            {
                ResListRoom resListRoom = new ResListRoom();
                resListRoom.Id = room.Id;
                resListRoom.RoomName = room.RoomName;

                resListRoom.SoLuong = roomService.getUserInRoomList(room.Id).Count;
                resListRooms.Add(resListRoom);
            }


            return Ok(new { status = true, message = resListRooms });
        }

        [HttpGet]
        [Route("getAllUserInRoom")]
        public IActionResult getAllUserInRoom(Guid roomId)
        {
            try
            {
                List<IdentityUser> list = new List<IdentityUser>();
                if (!cacheManage.userInRoom.ContainsKey(roomId.ToString().ToLower()))
                {
                    list = userInRoomService.getUserInRoom(roomId).ToList();
                    cacheManage.userInRoom.Add(roomId.ToString().ToLower(), list);
                }
                else
                {
                    list = cacheManage.userInRoom[roomId.ToString()];
                }

                return Ok(new { status = true, message = list });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getUserInRoom")]
        public IActionResult getUserInRoom(Guid roomId)
        {
            try
            {
                List<UserInRoom> userinroomList = userInRoomService.getAllUserInRoom(roomId);
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
            catch (Exception ex)
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
                List<IdentityUser> userinroomList = userInRoomService.getUserInRoom(roomId);
                if (!cacheManage.userInRoom.ContainsKey(roomId.ToString().ToLower()))
                {
                    //userinroomList = userInRoomService.getUserInRoom(roomId);
                    cacheManage.userInRoom.Add(roomId.ToString(), userinroomList);
                }
                //else
                //{
                //    cacheManage.userInRoom.Remove(roomId.ToString().ToLower());
                //    cacheManage.userInRoom.Add(roomId.ToString(), userinroomList);

                //}
                return Ok(new { status = true, message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex });
            }
        }
        [HttpDelete]
        [Route("deleteUserToRoom")]
        public IActionResult deleteUserToRoom(Guid roomId, string userId)
        {
            try
            {
                
                var user = userInRoomService.getUserInRoomById(roomId, userId);
                List<UserInRoom> listUser = userInRoomService.getAllUserInRoom(roomId);
                if (listUser.Count > 0)
                {

                    if (user.Role == "ChuPhong")
                    {
                        //lấy đối thủ làm chủ phòng
                        UserInRoom chuPhong = listUser.Where(u => u.Role == "DoiThu").FirstOrDefault();
                        if (chuPhong != null)
                        {
                            chuPhong.Role = "ChuPhong";
                            userInRoomService.updateUserInRoom(chuPhong);
                            //update
                        }
                        //lấy người đầu tiên trong danh sách làm đối thủ
                        UserInRoom nguoiXem = listUser.Where(u => u.Role == "NguoiXem").FirstOrDefault();
                        if (nguoiXem != null)
                        {
                            nguoiXem.Role = "DoiThu";
                            //update
                            userInRoomService.updateUserInRoom(nguoiXem);
                        }
                    }
                    else if (user.Role == "DoiThu")
                    {
                        //lấy người đầu tiên trong danh sách làm đối thủ
                        UserInRoom nguoiXem = listUser.Where(u => u.Role == "NguoiXem").FirstOrDefault();
                        if (nguoiXem != null)
                        {
                            nguoiXem.Role = "DoiThu";
                            //update
                            userInRoomService.updateUserInRoom(nguoiXem);
                        }
                    }
                }
                userInRoomService.delUserInRoom(roomId, userId);
                //todo: update list userInRoom trong cache
                List<IdentityUser> userinroomList = new List<IdentityUser>(); // tạo list user mới
                if (cacheManage.userInRoom.ContainsKey(roomId.ToString().ToLower()))
                {
                    userinroomList = userInRoomService.getUserInRoom(roomId);


                    if (userinroomList.Count == 0)
                    {
                        //    cacheManage.userInRoom.Remove(roomId.ToString().ToLower());
                        roomService.DeleteRoom(roomId);
                    }
                    else
                    {
                        //   cacheManage.userInRoom.Remove(roomId.ToString().ToLower());
                        //  cacheManage.userInRoom.Add(roomId.ToString(), userinroomList);
                    }

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
            catch (Exception ex)
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
