using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndGasApp.Dtos.User
{
    public class UserDto2Put
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int zGenderId { get; set; }
    }
}
