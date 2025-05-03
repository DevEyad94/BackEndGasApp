using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackEndGasApp.Constants;
using BackEndGasApp.Dtos.FieldMaintenance;
using BackEndGasApp.Dtos.ProductionRecord;
using BackEndGasApp.Extensions.Filters;
using BackEndGasApp.Services.GasService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BackEndGasApp.Controllers
{
    public class GasController : BaseApiController
    {
        private readonly IGasService _gasService;
        private readonly ILogger<GasController> _logger;

        public GasController(IGasService gasService, ILogger<GasController> logger)
        {
            _gasService = gasService;
            _logger = logger;
        }

        #region Field Maintenance Endpoints

        [HttpGet("fieldmaintenance/{id}")]
        [Authorize(Policy = Policies.AdminEngineerPolicy)]
        public async Task<
            ActionResult<ServiceResponse<GetFieldMaintenanceDto>>
        > GetFieldMaintenance(Guid id)
        {
            var result = await _gasService.GetFieldMaintenanceById(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("fieldmaintenance")]
        [Authorize(Policy = Policies.AdminEngineerPolicy)]
        public async Task<
            ActionResult<ServiceResponse<PagedList<GetFieldMaintenanceDto>>>
        > GetAllFieldMaintenances([FromQuery] Pagination pagination)
        {
            var data = await _gasService.GetAllFieldMaintenances(pagination);
            Response.AddPagination(
                data.Data.CurrentPage,
                data.Data.PageSize,
                data.Data.TotalCount,
                data.Data.TotalPages
            );
            return Ok(data);
        }

        [HttpGet("fieldmaintenance/filter")]
        [Authorize(Policy = Policies.AdminEngineerPolicy)]
        public async Task<
            ActionResult<ServiceResponse<PagedList<GetFieldMaintenanceDto>>>
        > GetFieldMaintenancesWithFilter(
            [FromQuery] Pagination pagination,
            [FromQuery] string search = null,
            [FromQuery] FieldMaintenanceFilter filter = null
        )
        {
            var data = await _gasService.GetFieldMaintenancesPagination(pagination, search, filter);
            Response.AddPagination(
                data.Data.CurrentPage,
                data.Data.PageSize,
                data.Data.TotalCount,
                data.Data.TotalPages
            );
            return Ok(data);
        }

        [HttpPost("fieldmaintenance")]
        [Authorize(Policy = Policies.AdminEngineerPolicy)]
        public async Task<
            ActionResult<ServiceResponse<GetFieldMaintenanceDto>>
        > AddFieldMaintenance(AddFieldMaintenanceDto newFieldMaintenance)
        {
            var result = await _gasService.AddFieldMaintenance(newFieldMaintenance);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("fieldmaintenance")]
        [Authorize(Policy = Policies.AdminEngineerPolicy)]
        public async Task<
            ActionResult<ServiceResponse<GetFieldMaintenanceDto>>
        > UpdateFieldMaintenance(UpdateFieldMaintenanceDto updateFieldMaintenance)
        {
            var result = await _gasService.UpdateFieldMaintenance(updateFieldMaintenance);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("fieldmaintenance/{id}")]
        [Authorize(Policy = Policies.AdminPolicy)]
        public async Task<
            ActionResult<ServiceResponse<List<GetFieldMaintenanceDto>>>
        > DeleteFieldMaintenance(Guid id)
        {
            var result = await _gasService.DeleteFieldMaintenance(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        #endregion

        #region Production Record Endpoints

        [HttpGet("productionrecord/{id}")]
        [Authorize(Policy = Policies.AdminOperatorPolicy)]
        public async Task<
            ActionResult<ServiceResponse<GetProductionRecordDto>>
        > GetProductionRecord(Guid id)
        {
            var result = await _gasService.GetProductionRecordById(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("productionrecord")]
        [Authorize(Policy = Policies.AdminOperatorPolicy)]
        public async Task<
            ActionResult<ServiceResponse<PagedList<GetProductionRecordDto>>>
        > GetAllProductionRecords([FromQuery] Pagination pagination)
        {
            var data = await _gasService.GetAllProductionRecords(pagination);
            Response.AddPagination(
                data.Data.CurrentPage,
                data.Data.PageSize,
                data.Data.TotalCount,
                data.Data.TotalPages
            );
            return Ok(data);
        }

        [HttpGet("productionrecord/filter")]
        [Authorize(Policy = Policies.AdminOperatorPolicy)]
        public async Task<
            ActionResult<ServiceResponse<PagedList<GetProductionRecordDto>>>
        > GetProductionRecordsWithFilter(
            [FromQuery] Pagination pagination,
            [FromQuery] string search = null,
            [FromQuery] ProductionRecordFilter filter = null
        )
        {
            var data = await _gasService.GetProductionRecordsPagination(pagination, search, filter);
            Response.AddPagination(
                data.Data.CurrentPage,
                data.Data.PageSize,
                data.Data.TotalCount,
                data.Data.TotalPages
            );
            return Ok(data);
        }

        [HttpPost("productionrecord")]
        [Authorize(Policy = Policies.AdminOperatorPolicy)]
        public async Task<
            ActionResult<ServiceResponse<GetProductionRecordDto>>
        > AddProductionRecord(AddProductionRecordDto newProductionRecord)
        {
            var result = await _gasService.AddProductionRecord(newProductionRecord);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("productionrecord")]
        [Authorize(Policy = Policies.AdminOperatorPolicy)]
        public async Task<
            ActionResult<ServiceResponse<GetProductionRecordDto>>
        > UpdateProductionRecord(UpdateProductionRecordDto updateProductionRecord)
        {
            var result = await _gasService.UpdateProductionRecord(updateProductionRecord);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("productionrecord/{id}")]
        [Authorize(Policy = Policies.AdminPolicy)]
        public async Task<
            ActionResult<ServiceResponse<List<GetProductionRecordDto>>>
        > DeleteProductionRecord(Guid id)
        {
            var result = await _gasService.DeleteProductionRecord(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("productionrecord/import")]
        [Authorize(Policy = Policies.AdminPolicy)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)] // 10MB limit
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<ActionResult<ServiceResponse<int>>> ImportProductionRecordsFromCsv(
            IFormFile file
        )
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(
                    new ServiceResponse<int> { Success = false, Message = "No file was uploaded" }
                );
            }

            if (
                !Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase)
            )
            {
                return BadRequest(
                    new ServiceResponse<int>
                    {
                        Success = false,
                        Message = "Only CSV files are supported",
                    }
                );
            }

            var result = await _gasService.ImportProductionRecordsFromCsv(file);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion
    }
}
