using AutoMapper;
using BackEndGasApp.Dtos.FieldMaintenance;
using BackEndGasApp.Dtos.ProductionRecord;
using BackEndGasApp.Models;

namespace BackEndGasApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Field Maintenance mappings
            CreateMap<Models.FieldMaintenance, GetFieldMaintenanceDto>()
                .ForMember(
                    dest => dest.MaintenanceTypeName,
                    opt => opt.MapFrom(src => src.zMaintenanceType.Name)
                )
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.zField.Name));

            CreateMap<AddFieldMaintenanceDto, Models.FieldMaintenance>();
            CreateMap<UpdateFieldMaintenanceDto, Models.FieldMaintenance>();

            // Production Record mappings
            CreateMap<Models.ProductionRecord, GetProductionRecordDto>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.zField.Name));

            CreateMap<AddProductionRecordDto, Models.ProductionRecord>();
            CreateMap<UpdateProductionRecordDto, Models.ProductionRecord>();
            CreateMap<ProductionRecordCsvImportDto, Models.ProductionRecord>();
        }
    }
}
