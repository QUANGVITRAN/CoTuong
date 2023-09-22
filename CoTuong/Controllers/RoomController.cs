using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoTuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private RoomService roomService;
        public RoomController (RoomService roomService)
        {
            this.roomService = roomService;
        }
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
        [Route("getRoomById")]
        public IActionResult getRoomById(Guid roomId)
        {
            Room room = roomService.getRoomById(roomId);
            return Ok(new { status = true, message = room });
        }
    }
}
