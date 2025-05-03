using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndGasApp.Dtos.User
{
    public class User2RegisterDto
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public int zRoleId { get; set; }
    }
}
