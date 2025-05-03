using AutoMapper;
using BackEndGasApp.Dtos.FieldMaintenance;
using BackEndGasApp.Dtos.ProductionRecord;
using BackEndGasApp.Dtos.User;
using BackEndGasApp.Models;
namespace BackEndGasApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<User2RegisterDto, User>();
            CreateMap<UserDto2Put, User>();
            CreateMap<UserRolePost, UserRole>();

            CreateMap<User, UserDto>()
                .ForMember(des => des.Roles, src => src.MapFrom(e => e.UserRoles));

            CreateMap<UserRole, RoleDto>()
                .ForMember(e => e.RoleName, opt => opt.MapFrom(ser => ser.zRole.Name));

            CreateMap<User2RegisterDto, User>();
            CreateMap<UserRolePost, UserRole>();
            CreateMap<UserDto2Put, User>();
            CreateMap<User, UserDto2Put>();

            // Field Maintenance mappings
            CreateMap<FieldMaintenance, GetFieldMaintenanceDto>()
                .ForMember(
                    dest => dest.MaintenanceTypeName,
                    opt => opt.MapFrom(src => src.zMaintenanceType.Name)
                )
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.zField.Name));

            CreateMap<AddFieldMaintenanceDto, FieldMaintenance>();
            CreateMap<UpdateFieldMaintenanceDto, FieldMaintenance>();
            CreateMap<zField, zFieldDTO>();

            // Production Record mappings
            CreateMap<ProductionRecord, GetProductionRecordDto>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.zField.Name));

            CreateMap<AddProductionRecordDto, ProductionRecord>();
            CreateMap<UpdateProductionRecordDto, ProductionRecord>();
            CreateMap<ProductionRecordCsvImportDto, ProductionRecord>();


            // zsk mappings
            CreateMap<zField, zFieldDTO>();
            CreateMap<zMaintenanceType, zMaintenanceTypeDTO>();
            
        }
    }
}
