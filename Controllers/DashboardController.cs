using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndGasApp.Dtos.Dashboard;
using BackEndGasApp.Services.DashboardService;
using Microsoft.AspNetCore.Authorization;

namespace BackEndGasApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : BaseApiController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IDashboardService _dashboardService;

        public DashboardController(ILogger<DashboardController> logger, IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetDashboardData([FromQuery] DashboardFilterDto filter)
        {
            var response = await _dashboardService.GetDashboardData(filter ?? new DashboardFilterDto());
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }
        
        [HttpGet("production-rate")]
        public async Task<IActionResult> GetProductionRateChart([FromQuery] DashboardFilterDto filter)
        {
            var response = await _dashboardService.GetProductionRateChart(filter ?? new DashboardFilterDto());
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }
        
        [HttpGet("maintenance-cost")]
        public async Task<IActionResult> GetMaintenanceCostChart([FromQuery] DashboardFilterDto filter)
        {
            var response = await _dashboardService.GetMaintenanceCostChart(filter ?? new DashboardFilterDto());
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }
        
        [HttpGet("region-distribution")]
        public async Task<IActionResult> GetRegionDistribution([FromQuery] DashboardFilterDto filter)
        {
            var response = await _dashboardService.GetRegionDistribution(filter ?? new DashboardFilterDto());
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }
        
        [HttpGet("field-data")]
        public async Task<IActionResult> GetFieldData([FromQuery] DashboardFilterDto filter)
        {
            var response = await _dashboardService.GetFieldData(filter ?? new DashboardFilterDto());
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }
        
        [HttpGet("field/{fieldId}")]
        public async Task<IActionResult> GetDashboardByField(int fieldId, [FromQuery] DashboardFilterDto filter)
        {
            var response = await _dashboardService.GetDashboardByField(fieldId);
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }
    }
}