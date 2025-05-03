using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackEndGasApp.Dtos.Dashboard;
using BackEndGasApp.Models;

namespace BackEndGasApp.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<ServiceResponse<DashboardResponseDto>> GetDashboardData(DashboardFilterDto filter);
        Task<ServiceResponse<List<ProductionRateChartDto>>> GetProductionRateChart(DashboardFilterDto filter);
        Task<ServiceResponse<List<MaintenanceCostChartDto>>> GetMaintenanceCostChart(DashboardFilterDto filter);
        Task<ServiceResponse<List<RegionDistributionDto>>> GetRegionDistribution(DashboardFilterDto filter);
        Task<ServiceResponse<List<FieldDataDto>>> GetFieldData(DashboardFilterDto filter);
    }
} 