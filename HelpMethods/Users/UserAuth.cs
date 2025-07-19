using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content_Management_System.Models;

namespace Content_Management_System.HelpMethods.Users
{
    public class UserAuth
    {
        private readonly UsersData _usersData;

        public UserAuth()
        {
            _usersData = new UsersData();
        }
        public bool UserAuthentication(string username, string password)
        {
            List<User> users = _usersData.DeSerializeObject<List<User>>("Users.xml");

            if (users == null)
                return false;

            foreach (var user in users)
            {
                if (user.Username == username && user.Password == password)
                    return true;
            }

            return false;
        }

    }
}
