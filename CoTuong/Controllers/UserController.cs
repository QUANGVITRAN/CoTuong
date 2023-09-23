using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoTuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserService userService;
        public UserController(UserService userService)
        {
            this.userService =userService;
        }
        [HttpPost]
        [Route("login")]
        public IActionResult login(string userName)
        {
            User user = new User();
            user.UserName = userName;
            user.Id = Guid.NewGuid();
            //user.RoomId = Guid.Empty;
            userService.Login(user);
            return Ok(new { status = true, message = userName });
        }

        
    }
}
