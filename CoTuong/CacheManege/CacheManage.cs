using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace CoTuong.CacheManege
{
    public class CacheManage
    {
        private IMemoryCache _memoryCache;
        private UserInRoomService _userInRoomService;
        private RoomService _roomService;

        public CacheManage(IMemoryCache memoryCache, UserInRoomService userInRoomService, RoomService roomService)
        {
            _memoryCache = memoryCache;
            _userInRoomService = userInRoomService;
            _roomService = roomService;
        }
        public Dictionary<string, List<IdentityUser>> userInRoom
        {
            get
            {
                Dictionary<string, List<IdentityUser>> result = (Dictionary<string, List<IdentityUser>>)_memoryCache.Get("memoryCache");
                if(result == null )
                {
                    result = new Dictionary<string, List<IdentityUser>>();
                    List<Room> rooms = _roomService.getAll();
                    foreach(Room room in rooms)
                    {
                        List<IdentityUser> userInRoomList = _userInRoomService.getUserInRoom(room.Id);
                        if (!result.ContainsKey(room.Id.ToString().ToLower()))
                        {
                            result.Add(room.Id.ToString().ToLower(), userInRoomList);
                        }                    
                    }
                    _memoryCache.Set("memoryCache", result);
                }   
                return result;
            }
        }
    }
}
