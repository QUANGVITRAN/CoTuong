using Libs.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Libs
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public DbSet<Room> Room { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserInRoom> UserInRoom { get; set; }

        public ApplicationDBContext (DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
    }
}