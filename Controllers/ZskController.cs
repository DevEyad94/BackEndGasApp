using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Constants;
using BackEndGasApp.Services.ZskService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndGasApp.Controllers
{
    public class ZskController : BaseApiController
    {
        private readonly IZskService zskService;

        public ZskController(IZskService zskService)
        {
            this.zskService = zskService;
        }

        [HttpGet("zRoles"), Authorize(Policy = Policies.AdminPolicy)]
        public async Task<ActionResult<ServiceResponse<zRoleDTO>>> getRoles()
        {
            return Ok(await zskService.getRoles());
        }

        [HttpGet("zFields"), Authorize]
        public async Task<ActionResult<ServiceResponse<zFieldDTO>>> getFields()
        {
            return Ok(await zskService.getFields());
        }

        [HttpGet("zMaintenanceTypes"), Authorize]
        public async Task<ActionResult<ServiceResponse<zMaintenanceTypeDTO>>> getMaintenanceTypes()
        {
            return Ok(await zskService.getMaintenanceTypes());
        }
    }
}
