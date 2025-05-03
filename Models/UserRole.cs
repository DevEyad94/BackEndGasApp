using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Models.zsk;

namespace BackEndGasApp.Models
{
    public class UserRole
    {
        public int UserRoleID { get; set; }
        public User User { get; set; }
        public int UserID { get; set; }
        public zRole zRole { get; set; }
        public int zRoleId { get; set; }
    }
}
