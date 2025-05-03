using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Dtos.User;

namespace BackEndGasApp.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> GetUser(string userName);
        Task<ServiceResponse<UserDto>> Login(string userName, string passwordHash);
        Task<ServiceResponse<UserDto2Put>> ModifyUser(UserDto2Put userDto2Put);

        Task<ServiceResponse<User>> NewUser(User2RegisterDto user2RegisterDto);

        Task<ServiceResponse<UserDto>> GetUserDataByToken();
        bool CheckUserByUserName(string userName);
        bool CheckUserByUserId(int id);
    }
}
