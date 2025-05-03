using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Data;
using BackEndGasApp.Dtos.Dashboard;
using BackEndGasApp.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using OfficeOpenXml;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace BackEndGasApp.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly DataContext _context;
        
        static DashboardService()
        {
            // Set EPPlus license for version 8.0 and higher
            ExcelPackage.License.SetNonCommercialPersonal("BackEndGasApp");
        }
        
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

        public async Task<ServiceResponse<byte[]>> ExportDashboardData(DashboardFilterDto filter, string format)
        {
            var response = new ServiceResponse<byte[]>();
            
            try
            {
                // First get all the dashboard data
                var dashboardResponse = await GetDashboardData(filter);
                if (!dashboardResponse.Success)
                {
                    response.Success = false;
                    response.Message = dashboardResponse.Message;
                    return response;
                }
                
                // Generate the appropriate file format
                if (format == "pdf")
                {
                    response.Data = GeneratePdfReport(dashboardResponse.Data);
                }
                else if (format == "excel")
                {
                    response.Data = GenerateExcelReport(dashboardResponse.Data);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Unsupported format. Please use 'pdf' or 'excel'.";
                    return response;
                }
                
                response.Success = true;
                response.Message = $"Dashboard data exported to {format.ToUpper()} successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to export dashboard data: {ex.Message}";
            }
            
            return response;
        }
        
        private byte[] GeneratePdfReport(DashboardResponseDto dashboard)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                
                // Get a bold font
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                
                // Add title
                document.Add(new Paragraph("Dashboard Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));
                
                document.Add(new Paragraph($"Generated on: {DateTime.Now}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(10));
                
                document.Add(new Paragraph("Summary")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(16)
                    .SetFont(boldFont));
                
                // Add summary data
                document.Add(new Paragraph($"Total Production Rate: {dashboard.TotalProductionRate} bbl/day")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12));
                
                document.Add(new Paragraph($"Total Maintenance Cost: ${dashboard.TotalMaintenanceCost}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12));
                
                // Add field data table
                document.Add(new Paragraph("Field Data")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(16)
                    .SetFont(boldFont));
                
                var table = new Table(5).UseAllAvailableWidth();
                table.AddHeaderCell("Field Name");
                table.AddHeaderCell("Latitude");
                table.AddHeaderCell("Longitude");
                table.AddHeaderCell("Production Rate (bbl/day)");
                table.AddHeaderCell("Maintenance Cost ($)");
                
                foreach (var field in dashboard.FieldData)
                {
                    table.AddCell(field.FieldName);
                    table.AddCell(field.Latitude.ToString());
                    table.AddCell(field.Longitude.ToString());
                    table.AddCell(field.ProductionRate.ToString());
                    table.AddCell(field.MaintenanceCost.ToString());
                }
                
                document.Add(table);
                
                // Add production rate chart data
                document.Add(new Paragraph("Production Rate by Period")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(16)
                    .SetFont(boldFont));
                
                var productionTable = new Table(2).UseAllAvailableWidth();
                productionTable.AddHeaderCell("Period");
                productionTable.AddHeaderCell("Production Rate (bbl/day)");
                
                foreach (var data in dashboard.ProductionRateChart)
                {
                    productionTable.AddCell(data.Period);
                    productionTable.AddCell(data.ProductionRate.ToString());
                }
                
                document.Add(productionTable);
                
                // Add maintenance cost chart data
                document.Add(new Paragraph("Maintenance Cost by Period")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(16)
                    .SetFont(boldFont));
                
                var maintenanceTable = new Table(2).UseAllAvailableWidth();
                maintenanceTable.AddHeaderCell("Period");
                maintenanceTable.AddHeaderCell("Maintenance Cost ($)");
                
                foreach (var data in dashboard.MaintenanceCostChart)
                {
                    maintenanceTable.AddCell(data.Period);
                    maintenanceTable.AddCell(data.Cost.ToString());
                }
                
                document.Add(maintenanceTable);
                
                // Add region distribution
                document.Add(new Paragraph("Region Distribution")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(16)
                    .SetFont(boldFont));
                
                var regionTable = new Table(2).UseAllAvailableWidth();
                regionTable.AddHeaderCell("Region");
                regionTable.AddHeaderCell("Field Count");
                
                foreach (var data in dashboard.RegionDistribution)
                {
                    regionTable.AddCell(data.RegionName);
                    regionTable.AddCell(data.FieldCount.ToString());
                }
                
                document.Add(regionTable);
                
                document.Close();
                return memoryStream.ToArray();
            }
        }
        
        private byte[] GenerateExcelReport(DashboardResponseDto dashboard)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    // Summary worksheet
                    var summaryWorksheet = package.Workbook.Worksheets.Add("Summary");
                    summaryWorksheet.Cells[1, 1].Value = "Dashboard Report";
                    summaryWorksheet.Cells[1, 1].Style.Font.Size = 20;
                    summaryWorksheet.Cells[1, 1].Style.Font.Bold = true;
                    
                    summaryWorksheet.Cells[2, 1].Value = $"Generated on: {DateTime.Now}";
                    
                    summaryWorksheet.Cells[4, 1].Value = "Total Production Rate:";
                    summaryWorksheet.Cells[4, 2].Value = dashboard.TotalProductionRate;
                    
                    summaryWorksheet.Cells[5, 1].Value = "Total Maintenance Cost:";
                    summaryWorksheet.Cells[5, 2].Value = dashboard.TotalMaintenanceCost;
                    
                    // Field data worksheet
                    var fieldWorksheet = package.Workbook.Worksheets.Add("Field Data");
                    fieldWorksheet.Cells[1, 1].Value = "Field Name";
                    fieldWorksheet.Cells[1, 2].Value = "Latitude";
                    fieldWorksheet.Cells[1, 3].Value = "Longitude";
                    fieldWorksheet.Cells[1, 4].Value = "Production Rate (bbl/day)";
                    fieldWorksheet.Cells[1, 5].Value = "Maintenance Cost ($)";
                    
                    // Style header row
                    for (int i = 1; i <= 5; i++)
                    {
                        fieldWorksheet.Cells[1, i].Style.Font.Bold = true;
                    }
                    
                    // Add field data
                    for (int i = 0; i < dashboard.FieldData.Count; i++)
                    {
                        var field = dashboard.FieldData[i];
                        fieldWorksheet.Cells[i + 2, 1].Value = field.FieldName;
                        fieldWorksheet.Cells[i + 2, 2].Value = field.Latitude;
                        fieldWorksheet.Cells[i + 2, 3].Value = field.Longitude;
                        fieldWorksheet.Cells[i + 2, 4].Value = field.ProductionRate;
                        fieldWorksheet.Cells[i + 2, 5].Value = field.MaintenanceCost;
                    }
                    
                    fieldWorksheet.Cells[fieldWorksheet.Dimension.Address].AutoFitColumns();
                    
                    // Production rate worksheet
                    var productionWorksheet = package.Workbook.Worksheets.Add("Production Rate");
                    productionWorksheet.Cells[1, 1].Value = "Period";
                    productionWorksheet.Cells[1, 2].Value = "Production Rate (bbl/day)";
                    
                    // Style header row
                    productionWorksheet.Cells[1, 1].Style.Font.Bold = true;
                    productionWorksheet.Cells[1, 2].Style.Font.Bold = true;
                    
                    // Add production rate data
                    for (int i = 0; i < dashboard.ProductionRateChart.Count; i++)
                    {
                        var data = dashboard.ProductionRateChart[i];
                        productionWorksheet.Cells[i + 2, 1].Value = data.Period;
                        productionWorksheet.Cells[i + 2, 2].Value = data.ProductionRate;
                    }
                    
                    productionWorksheet.Cells[productionWorksheet.Dimension.Address].AutoFitColumns();
                    
                    // Maintenance cost worksheet
                    var maintenanceWorksheet = package.Workbook.Worksheets.Add("Maintenance Cost");
                    maintenanceWorksheet.Cells[1, 1].Value = "Period";
                    maintenanceWorksheet.Cells[1, 2].Value = "Maintenance Cost ($)";
                    
                    // Style header row
                    maintenanceWorksheet.Cells[1, 1].Style.Font.Bold = true;
                    maintenanceWorksheet.Cells[1, 2].Style.Font.Bold = true;
                    
                    // Add maintenance cost data
                    for (int i = 0; i < dashboard.MaintenanceCostChart.Count; i++)
                    {
                        var data = dashboard.MaintenanceCostChart[i];
                        maintenanceWorksheet.Cells[i + 2, 1].Value = data.Period;
                        maintenanceWorksheet.Cells[i + 2, 2].Value = data.Cost;
                    }
                    
                    maintenanceWorksheet.Cells[maintenanceWorksheet.Dimension.Address].AutoFitColumns();
                    
                    // Region distribution worksheet
                    var regionWorksheet = package.Workbook.Worksheets.Add("Region Distribution");
                    regionWorksheet.Cells[1, 1].Value = "Region";
                    regionWorksheet.Cells[1, 2].Value = "Field Count";
                    
                    // Style header row
                    regionWorksheet.Cells[1, 1].Style.Font.Bold = true;
                    regionWorksheet.Cells[1, 2].Style.Font.Bold = true;
                    
                    // Add region distribution data
                    for (int i = 0; i < dashboard.RegionDistribution.Count; i++)
                    {
                        var data = dashboard.RegionDistribution[i];
                        regionWorksheet.Cells[i + 2, 1].Value = data.RegionName;
                        regionWorksheet.Cells[i + 2, 2].Value = data.FieldCount;
                    }
                    
                    regionWorksheet.Cells[regionWorksheet.Dimension.Address].AutoFitColumns();
                    
                    package.Save();
                }
                
                return memoryStream.ToArray();
            }
        }
    }
} 