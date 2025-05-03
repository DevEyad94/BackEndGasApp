using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Models.zsk;
namespace BackEndGasApp.Services.ZskService
{
    public interface IZskService
    {
        Task<ServiceResponse<List<zFieldDTO>>> getFields();
        Task<ServiceResponse<List<zMaintenanceTypeDTO>>> getMaintenanceTypes();
        Task<ServiceResponse<List<zRoleDTO>>> getRoles();
    }
}
