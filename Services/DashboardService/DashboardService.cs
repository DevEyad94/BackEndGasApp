using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Data;
using BackEndGasApp.Dtos.Dashboard;
using BackEndGasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndGasApp.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly DataContext _context;
        
        public DashboardService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<DashboardResponseDto>> GetDashboardData(DashboardFilterDto filter)
        {
            var response = new ServiceResponse<DashboardResponseDto>();
            
            try
            {
                var dashboard = new DashboardResponseDto();
                
                // Check if we have a specific field ID filter
                if (filter.FieldId.HasValue)
                {
                    // Verify field exists
                    var field = await _context.zFields.FirstOrDefaultAsync(f => f.zFieldId == filter.FieldId.Value);
                    if (field == null)
                    {
                        response.Success = false;
                        response.Message = $"Field with ID {filter.FieldId.Value} not found.";
                        return response;
                    }
                }
                
                // Get production rate data
                var productionRateResponse = await GetProductionRateChart(filter);
                if (productionRateResponse.Success)
                    dashboard.ProductionRateChart = productionRateResponse.Data;
                
                // Get maintenance cost data
                var maintenanceCostResponse = await GetMaintenanceCostChart(filter);
                if (maintenanceCostResponse.Success)
                    dashboard.MaintenanceCostChart = maintenanceCostResponse.Data;
                
                // Get region distribution
                var regionDistributionResponse = await GetRegionDistribution(filter);
                if (regionDistributionResponse.Success)
                    dashboard.RegionDistribution = regionDistributionResponse.Data;
                
                // Get field data
                var fieldDataResponse = await GetFieldData(filter);
                if (fieldDataResponse.Success)
                    dashboard.FieldData = fieldDataResponse.Data;
                
                // Calculate totals
                dashboard.TotalProductionRate = dashboard.FieldData.Sum(f => f.ProductionRate);
                dashboard.TotalMaintenanceCost = dashboard.FieldData.Sum(f => f.MaintenanceCost);
                
                // Set appropriate message based on filter
                string message = "Dashboard data retrieved successfully.";
                if (filter.FieldId.HasValue)
                {
                    var fieldName = dashboard.FieldData.FirstOrDefault(f => f.FieldId == filter.FieldId.Value)?.FieldName;
                    message = $"Dashboard data for field '{fieldName ?? filter.FieldId.Value.ToString()}' retrieved successfully.";
                }
                
                response.Data = dashboard;
                response.Success = true;
                response.Message = message;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public async Task<ServiceResponse<FieldDashboardDto>> GetDashboardByField(int fieldId)
        {
            var response = new ServiceResponse<FieldDashboardDto>();
            
            try
            {
                // Create filter with field ID
                var filter = new DashboardFilterDto { FieldId = fieldId };
                
                var field = await _context.zFields
                    .FirstOrDefaultAsync(f => f.zFieldId == fieldId);
                    
                if (field == null)
                {
                    response.Success = false;
                    response.Message = $"Field with ID {fieldId} not found.";
                    return response;
                }
                
                var dashboard = new FieldDashboardDto
                {
                    FieldId = field.zFieldId,
                    FieldName = field.Name,
                    Latitude = (double)field.Latitude,
                    Longitude = (double)field.Longitude
                };
                
                // Get production rate data
                var productionRateResponse = await GetProductionRateChart(filter);
                if (productionRateResponse.Success)
                    dashboard.ProductionRateChart = productionRateResponse.Data;
                
                // Get maintenance cost data
                var maintenanceCostResponse = await GetMaintenanceCostChart(filter);
                if (maintenanceCostResponse.Success)
                    dashboard.MaintenanceCostChart = maintenanceCostResponse.Data;
                
                // Calculate totals
                dashboard.TotalProductionRate = dashboard.ProductionRateChart?.Sum(p => p.ProductionRate) ?? 0;
                dashboard.TotalMaintenanceCost = dashboard.MaintenanceCostChart?.Sum(m => m.Cost) ?? 0;
                
                response.Data = dashboard;
                response.Success = true;
                response.Message = $"Dashboard data for field '{field.Name}' retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public async Task<ServiceResponse<List<ProductionRateChartDto>>> GetProductionRateChart(DashboardFilterDto filter)
        {
            var response = new ServiceResponse<List<ProductionRateChartDto>>();
            
            try
            {
                var query = _context.ProductionRecords.AsQueryable();
                
                // Apply filters
                ApplyProductionRecordFilters(ref query, filter);
                
                // Get the raw data first to avoid string interpolation in the SQL translation
                var rawData = await query
                    .Select(p => new 
                    {
                        Month = p.DateOfProduction.Month,
                        Year = p.DateOfProduction.Year,
                        ProductionRate = p.ProductionRate
                    })
                    .ToListAsync();
                    
                // Group and format the data in memory
                var groupedData = rawData
                    .GroupBy(p => new { p.Month, p.Year })
                    .Select(g => new ProductionRateChartDto
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month}",
                        ProductionRate = g.Sum(p => p.ProductionRate)
                    })
                    .OrderBy(p => p.Period)
                    .ToList();
                
                response.Data = groupedData;
                response.Success = true;
                response.Message = "Production rate chart data retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public async Task<ServiceResponse<List<MaintenanceCostChartDto>>> GetMaintenanceCostChart(DashboardFilterDto filter)
        {
            var response = new ServiceResponse<List<MaintenanceCostChartDto>>();
            
            try
            {
                var query = _context.FieldMaintenances.AsQueryable();
                
                // Apply filters
                ApplyFieldMaintenanceFilters(ref query, filter);
                
                // Get the raw data first to avoid string interpolation in the SQL translation
                var rawData = await query
                    .Select(m => new 
                    {
                        Month = m.FieldMaintenanceDate.Month,
                        Year = m.FieldMaintenanceDate.Year,
                        Cost = m.Cost
                    })
                    .ToListAsync();
                    
                // Group and format the data in memory
                var groupedData = rawData
                    .GroupBy(m => new { m.Month, m.Year })
                    .Select(g => new MaintenanceCostChartDto
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month}",
                        Cost = g.Sum(m => m.Cost)
                    })
                    .OrderBy(m => m.Period)
                    .ToList();
                
                response.Data = groupedData;
                response.Success = true;
                response.Message = "Maintenance cost chart data retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public async Task<ServiceResponse<List<RegionDistributionDto>>> GetRegionDistribution(DashboardFilterDto filter)
        {
            var response = new ServiceResponse<List<RegionDistributionDto>>();
            
            try
            {
                // For now, we'll just count fields per region
                // In a real application, you might have a region property on the field model
                var query = _context.zFields.AsQueryable();
                
                // Apply field ID filter if provided
                if (filter.FieldId.HasValue)
                {
                    query = query.Where(f => f.zFieldId == filter.FieldId.Value);
                }
                
                var fieldData = await query
                    .Select(f => new { f.zFieldId, RegionName = "Default Region" }) // Replace with actual region data if available
                    .GroupBy(f => f.RegionName)
                    .Select(g => new RegionDistributionDto
                    {
                        RegionName = g.Key,
                        FieldCount = g.Count()
                    })
                    .ToListAsync();
                
                response.Data = fieldData;
                response.Success = true;
                response.Message = "Region distribution data retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public async Task<ServiceResponse<List<FieldDataDto>>> GetFieldData(DashboardFilterDto filter)
        {
            var response = new ServiceResponse<List<FieldDataDto>>();
            
            try
            {
                // Query for production records with filters
                var productionQuery = _context.ProductionRecords
                    .Include(p => p.zField)
                    .AsQueryable();
                
                // Apply production record filters
                ApplyProductionRecordFilters(ref productionQuery, filter);
                
                // Query for maintenance records with filters
                var maintenanceQuery = _context.FieldMaintenances
                    .Include(m => m.zField)
                    .Include(m => m.zMaintenanceType)
                    .AsQueryable();
                
                // Apply maintenance filters
                ApplyFieldMaintenanceFilters(ref maintenanceQuery, filter);
                
                // Get unique field IDs from both filtered queries
                var productionFieldIds = await productionQuery.Select(p => p.zFieldId).Distinct().ToListAsync();
                var maintenanceFieldIds = await maintenanceQuery.Select(m => m.zFieldId).Distinct().ToListAsync();
                
                // Union of both sets of field IDs
                var fieldIds = productionFieldIds.Union(maintenanceFieldIds).Distinct().ToList();
                
                // Get field data for these IDs
                var fieldsQuery = _context.zFields
                    .Where(f => fieldIds.Contains(f.zFieldId));
                
                // If field ID filter is directly provided, apply it
                if (filter.FieldId.HasValue && !fieldIds.Contains(filter.FieldId.Value))
                {
                    fieldsQuery = fieldsQuery.Union(_context.zFields.Where(f => f.zFieldId == filter.FieldId.Value));
                }
                
                var fields = await fieldsQuery.ToListAsync();
                
                var result = new List<FieldDataDto>();
                
                // For each field, get the latest production and maintenance data
                foreach (var field in fields)
                {
                    var latestProduction = await productionQuery
                        .Where(p => p.zFieldId == field.zFieldId)
                        .OrderByDescending(p => p.DateOfProduction)
                        .FirstOrDefaultAsync();
                    
                    var latestMaintenance = await maintenanceQuery
                        .Where(m => m.zFieldId == field.zFieldId)
                        .OrderByDescending(m => m.FieldMaintenanceDate)
                        .FirstOrDefaultAsync();
                    
                    if (latestProduction != null || latestMaintenance != null || filter.FieldId.HasValue)
                    {
                        result.Add(new FieldDataDto
                        {
                            FieldId = field.zFieldId,
                            FieldName = field.Name,
                            Latitude = field.Latitude,
                            Longitude = field.Longitude,
                            ProductionRate = latestProduction?.ProductionRate ?? 0,
                            ExtractionYear = latestProduction?.DateOfProduction.Year ?? DateTime.Now.Year,
                            MaintenanceType = latestMaintenance?.zMaintenanceType?.Name ?? "None",
                            MaintenanceCost = latestMaintenance?.Cost ?? 0,
                            LastMaintenanceDate = latestMaintenance?.FieldMaintenanceDate ?? DateTime.MinValue
                        });
                    }
                }
                
                response.Data = result;
                response.Success = true;
                response.Message = "Field data retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }
        
        private void ApplyProductionRecordFilters(ref IQueryable<ProductionRecord> query, DashboardFilterDto filter)
        {
            if (filter.MinProductionRate.HasValue)
                query = query.Where(p => p.ProductionRate >= filter.MinProductionRate.Value);
                
            if (filter.MaxProductionRate.HasValue)
                query = query.Where(p => p.ProductionRate <= filter.MaxProductionRate.Value);
                
            if (filter.ExtractionYear.HasValue)
                query = query.Where(p => p.DateOfProduction.Year == filter.ExtractionYear.Value);
                
            if (filter.FromYear.HasValue)
                query = query.Where(p => p.DateOfProduction.Year >= filter.FromYear.Value);
                
            if (filter.ToYear.HasValue)
                query = query.Where(p => p.DateOfProduction.Year <= filter.ToYear.Value);
                
            if (filter.FieldId.HasValue)
                query = query.Where(p => p.zFieldId == filter.FieldId.Value);
        }
        
        private void ApplyFieldMaintenanceFilters(ref IQueryable<FieldMaintenance> query, DashboardFilterDto filter)
        {
            if (filter.MaintenanceTypeId.HasValue)
                query = query.Where(m => m.zMaintenanceTypeId == filter.MaintenanceTypeId.Value);
                
            if (filter.MinCost.HasValue)
                query = query.Where(m => m.Cost >= filter.MinCost.Value);
                
            if (filter.MaxCost.HasValue)
                query = query.Where(m => m.Cost <= filter.MaxCost.Value);
                
            if (filter.FromYear.HasValue)
                query = query.Where(m => m.FieldMaintenanceDate.Year >= filter.FromYear.Value);
                
            if (filter.ToYear.HasValue)
                query = query.Where(m => m.FieldMaintenanceDate.Year <= filter.ToYear.Value);
                
            if (filter.FieldId.HasValue)
                query = query.Where(m => m.zFieldId == filter.FieldId.Value);
        }
    }
} 