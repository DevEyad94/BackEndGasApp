using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Services.DatabaseService;

namespace BackEndGasApp.Services.ZskService
{
    public class ZskService : IZskService
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly DataContext context;
        private readonly IDatabaseService databaseService;

        public ZskService(
            IDatabaseService databaseService,
            DataContext context,
            IMapper mapper,
            IConfiguration configuration
        )
        {
            this.databaseService = databaseService;
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public async Task<ServiceResponse<List<zFieldDTO>>> getFields()
        {
            var serviceResponse = new ServiceResponse<List<zFieldDTO>>();
            var dataFromDB = await mapper
                .ProjectTo<zFieldDTO>(context.zFields, mapper.ConfigurationProvider)
                .ToListAsync();
            if (dataFromDB is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Not found!";
            }
            serviceResponse.Data = dataFromDB;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<zMaintenanceTypeDTO>>> getMaintenanceTypes()
        {
            var serviceResponse = new ServiceResponse<List<zMaintenanceTypeDTO>>();
            var dataFromDB = await mapper
                .ProjectTo<zMaintenanceTypeDTO>(
                    context.zMaintenanceTypes,
                    mapper.ConfigurationProvider
                )
                .ToListAsync();
            if (dataFromDB is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Not found!";
            }
            serviceResponse.Data = dataFromDB;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<zRoleDTO>>> getRoles()
        {
            var serviceResponse = new ServiceResponse<List<zRoleDTO>>();
            var dataFromDB = await mapper
                .ProjectTo<zRoleDTO>(context.zRoles, mapper.ConfigurationProvider)
                .ToListAsync();
            if (dataFromDB is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Not found!";
            }
            serviceResponse.Data = dataFromDB;
            return serviceResponse;
        }
    }
}
