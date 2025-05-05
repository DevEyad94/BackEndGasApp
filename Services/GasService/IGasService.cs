using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BackEndGasApp.Extensions.Filters;
using BackEndGasApp.Dtos.FieldMaintenance;
using BackEndGasApp.Dtos.ProductionRecord;

namespace BackEndGasApp.Services.GasService
{
    public interface IGasService
    {
        // Field Maintenance operations
        Task<ServiceResponse<PagedList<GetFieldMaintenanceDto>>> GetAllFieldMaintenances(Pagination pagination);
        Task<ServiceResponse<PagedList<GetFieldMaintenanceDto>>> GetFieldMaintenancesPagination(
            Pagination pagination,
            string search = null,
            FieldMaintenanceFilter fieldMaintenanceFilter = null
        );
        Task<ServiceResponse<GetFieldMaintenanceDto>> GetFieldMaintenanceById(Guid id);
        Task<bool> CheckFieldMaintenanceById(Guid id);
        Task<ServiceResponse<GetFieldMaintenanceDto>> AddFieldMaintenance(AddFieldMaintenanceDto newFieldMaintenance);
        Task<ServiceResponse<GetFieldMaintenanceDto>> UpdateFieldMaintenance(UpdateFieldMaintenanceDto updateFieldMaintenance);
        Task<ServiceResponse<List<GetFieldMaintenanceDto>>> DeleteFieldMaintenance(Guid id);

        // Production Record operations
        Task<ServiceResponse<PagedList<GetProductionRecordDto>>> GetAllProductionRecords(Pagination pagination);
        Task<ServiceResponse<PagedList<GetProductionRecordDto>>> GetProductionRecordsPagination(
            Pagination pagination,
            string search = null,
            ProductionRecordFilter productionRecordFilter = null
        );
        Task<ServiceResponse<GetProductionRecordDto>> GetProductionRecordById(Guid id);
        Task<bool> CheckProductionRecordById(Guid id);
        Task<ServiceResponse<GetProductionRecordDto>> AddProductionRecord(AddProductionRecordDto newProductionRecord);
        Task<ServiceResponse<GetProductionRecordDto>> UpdateProductionRecord(UpdateProductionRecordDto updateProductionRecord);
        Task<ServiceResponse<List<GetProductionRecordDto>>> DeleteProductionRecord(Guid id);
        Task<ServiceResponse<int>> ImportProductionRecordsFromCsv(IFormFile csvFile);
        
        // New method to get years with their disabled months
        Task<ServiceResponse<List<YearDisabledMonthsDto>>> GetDisabledMonths(int? fieldId = null);
    }
}
