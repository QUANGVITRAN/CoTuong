using Libs.Entity;
using Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Services
{
    public class UserService
    {
        private ApplicationDBContext dbContext;
        private UserRepository userRepository;

        public UserService(ApplicationDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.userRepository = new UserRepository(dbContext);
        }
        public void Login(User user)
        {
            userRepository.Add(user);
            Save();
        }
        public void Save()
        {
            userRepository.SaveChanges();
        }
    }
}
