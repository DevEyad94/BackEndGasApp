global using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using BackEndGasApp.Attributes;
using BackEndGasApp.Constants;
using BackEndGasApp.Dtos.FieldMaintenance;
using BackEndGasApp.Dtos.ProductionRecord;
using BackEndGasApp.Extensions;
using BackEndGasApp.Extensions.Filters;
using BackEndGasApp.Helpers;
using BackEndGasApp.Models;
using BackEndGasApp.Services.DatabaseService;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;

namespace BackEndGasApp.Services.GasService
{
    public class GasService : IGasService
    {
        private readonly IMapper mapper;
        private readonly DataContext context;
        private readonly IDatabaseService databaseService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GasService(
            IMapper mapper,
            DataContext context,
            IDatabaseService databaseService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this.mapper = mapper;
            this.context = context;
            this.databaseService = databaseService;
            this.httpContextAccessor = httpContextAccessor;
        }

        #region Field Maintenance
        public async Task<ServiceResponse<GetFieldMaintenanceDto>> AddFieldMaintenance(
            AddFieldMaintenanceDto newFieldMaintenance
        )
        {
            var serviceResponse = new ServiceResponse<GetFieldMaintenanceDto>();
            var currentUserId = httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.UserData)
                ?.Value;

            // Add 8 hours and ensure UTC
            var adjustedDate = DateTime.SpecifyKind(
                newFieldMaintenance.FieldMaintenanceDate.AddHours(8),
                DateTimeKind.Utc
            );
            
            // Create the start and end of month in UTC
            var startOfMonth = new DateTime(adjustedDate.Year, adjustedDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1);

            var existingRecord = await context.FieldMaintenances
                .AnyAsync(p => 
                    p.zFieldId == newFieldMaintenance.zFieldId &&
                    p.FieldMaintenanceDate >= startOfMonth &&
                    p.FieldMaintenanceDate < endOfMonth
                );

            if (existingRecord)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"A field maintenance record already exists for {adjustedDate:MMMM yyyy} for this field.";
                return serviceResponse;
            }

            // Map DTO to entity
            var fieldMaintenanceEntity = mapper.Map<FieldMaintenance>(newFieldMaintenance);

            // Convert DateTime properties to UTC
            fieldMaintenanceEntity.FieldMaintenanceDate = adjustedDate;
            fieldMaintenanceEntity.CreatedBy = currentUserId;

            databaseService.add(fieldMaintenanceEntity);

            if (await databaseService.SaveAll())
            {
                var resultEntity = await context
                    .FieldMaintenances.Include(f => f.zMaintenanceType)
                    .Include(f => f.zField)
                    .FirstOrDefaultAsync(e =>
                        e.FieldMaintenanceGuid == fieldMaintenanceEntity.FieldMaintenanceGuid
                    );

                serviceResponse.Data = mapper.Map<GetFieldMaintenanceDto>(resultEntity);
                return serviceResponse;
            }

            serviceResponse.Success = false;
            serviceResponse.Message = "Error in Adding Data";
            return serviceResponse;
        }

        public async Task<bool> CheckFieldMaintenanceById(Guid id)
        {
            return await context.FieldMaintenances.AnyAsync(e => e.FieldMaintenanceGuid == id);
        }

        public async Task<ServiceResponse<List<GetFieldMaintenanceDto>>> DeleteFieldMaintenance(
            Guid id
        )
        {
            var serviceResponse = new ServiceResponse<List<GetFieldMaintenanceDto>>();
            try
            {
                var fieldMaintenance = await context.FieldMaintenances.FirstOrDefaultAsync(c =>
                    c.FieldMaintenanceGuid == id
                );
                if (fieldMaintenance is null)
                    throw new Exception($"Field Maintenance with ID '{id}' not found.");

                context.FieldMaintenances.Remove(fieldMaintenance);
                await context.SaveChangesAsync();

                var fieldMaintenanceEntities = await context
                    .FieldMaintenances.Include(f => f.zMaintenanceType)
                    .Include(f => f.zField)
                    .ToListAsync();

                serviceResponse.Data = mapper.Map<List<GetFieldMaintenanceDto>>(
                    fieldMaintenanceEntities
                );
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.Message;
                serviceResponse.Success = false;
            }

            return serviceResponse;
        }

        public async Task<
            ServiceResponse<PagedList<GetFieldMaintenanceDto>>
        > GetAllFieldMaintenances(Pagination pagination)
        {
            // Null check for parameter
            pagination ??= new Pagination();

            var query = context
                .FieldMaintenances.Include(f => f.zMaintenanceType)
                .Include(f => f.zField)
                .AsQueryable();

            var projectedQuery = query.ProjectTo<GetFieldMaintenanceDto>(
                mapper.ConfigurationProvider
            );

            var pagedData = await Task.FromResult(
                PagedList<GetFieldMaintenanceDto>.ToPagedList(
                    projectedQuery,
                    pagination.PageNumber,
                    pagination.PageSize
                )
            );

            var serviceResponse = new ServiceResponse<PagedList<GetFieldMaintenanceDto>>
            {
                Data = pagedData,
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetFieldMaintenanceDto>> GetFieldMaintenanceById(Guid id)
        {
            var serviceResponse = new ServiceResponse<GetFieldMaintenanceDto>();
            var fieldMaintenance = await context
                .FieldMaintenances.Include(f => f.zMaintenanceType)
                .Include(f => f.zField)
                .FirstOrDefaultAsync(e => e.FieldMaintenanceGuid == id);

            if (fieldMaintenance is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Field Maintenance not found!";
                return serviceResponse;
            }

            serviceResponse.Data = mapper.Map<GetFieldMaintenanceDto>(fieldMaintenance);
            return serviceResponse;
        }

        public async Task<
            ServiceResponse<PagedList<GetFieldMaintenanceDto>>
        > GetFieldMaintenancesPagination(
            Pagination pagination,
            string search = null,
            FieldMaintenanceFilter fieldMaintenanceFilter = null
        )
        {
            // Null checks for parameters
            pagination ??= new Pagination();
            fieldMaintenanceFilter ??= new FieldMaintenanceFilter();

            // Ensure required fields have default values
            pagination.SortColumn ??= "FieldMaintenanceDate"; // Default sort column
            pagination.SortDirection ??= "desc"; // Default sort direction

            // Start with a base query
            var query = context
                .FieldMaintenances.Include(f => f.zMaintenanceType)
                .Include(f => f.zField)
                .AsQueryable();

            // Apply filters
            if (fieldMaintenanceFilter.zMaintenanceTypeId.HasValue)
                query = query.Where(e =>
                    e.zMaintenanceTypeId == fieldMaintenanceFilter.zMaintenanceTypeId.Value
                );

            if (fieldMaintenanceFilter.zFieldId.HasValue)
                query = query.Where(e => e.zFieldId == fieldMaintenanceFilter.zFieldId.Value);

            if (fieldMaintenanceFilter.StartDate.HasValue)
            {
                var startDate = fieldMaintenanceFilter.StartDate.Value.ToUniversalTime();
                query = query.Where(e => e.FieldMaintenanceDate >= startDate);
            }

            if (fieldMaintenanceFilter.EndDate.HasValue)
            {
                var endDate = fieldMaintenanceFilter.EndDate.Value.ToUniversalTime().AddDays(1);
                query = query.Where(e => e.FieldMaintenanceDate < endDate);
            }

            // Apply cost filters
            if (fieldMaintenanceFilter.MinCost.HasValue)
                query = query.Where(e => e.Cost >= fieldMaintenanceFilter.MinCost.Value);

            if (fieldMaintenanceFilter.MaxCost.HasValue)
                query = query.Where(e => e.Cost <= fieldMaintenanceFilter.MaxCost.Value);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => (x.Description != null && x.Description.Contains(search)));
            }

            // Apply ordering
            query = ApplySorting(query, pagination.SortColumn, pagination.SortDirection);

            // Project to DTO and apply pagination
            var projectedQuery = query.ProjectTo<GetFieldMaintenanceDto>(
                mapper.ConfigurationProvider
            );

            var pagedData = await Task.FromResult(
                PagedList<GetFieldMaintenanceDto>.ToPagedList(
                    projectedQuery,
                    pagination.PageNumber,
                    pagination.PageSize
                )
            );

            return new ServiceResponse<PagedList<GetFieldMaintenanceDto>>
            {
                Data = pagedData,
                Success = true,
            };
        }

        public async Task<ServiceResponse<GetFieldMaintenanceDto>> UpdateFieldMaintenance(
            UpdateFieldMaintenanceDto updateFieldMaintenance
        )
        {
            var serviceResponse = new ServiceResponse<GetFieldMaintenanceDto>();
            var currentUserId = httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.UserData)
                ?.Value;

            var existingEntity = await context.FieldMaintenances.FindAsync(
                updateFieldMaintenance.FieldMaintenanceGuid
            );

            if (existingEntity == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Field Maintenance not found!";
                return serviceResponse;
            }

            // Add 8 hours and ensure UTC for the new date
            var adjustedNewDate = DateTime.SpecifyKind(
                updateFieldMaintenance.FieldMaintenanceDate.AddHours(8),
                DateTimeKind.Utc
            );

            // Check if the month is being changed
            if (existingEntity.FieldMaintenanceDate.Year != adjustedNewDate.Year ||
                existingEntity.FieldMaintenanceDate.Month != adjustedNewDate.Month)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Cannot change the month of an existing field maintenance record.";
                return serviceResponse;
            }

            existingEntity.UpdatedBy = currentUserId;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            // Map DTO to entity, preserving the ID and audit fields
            mapper.Map(updateFieldMaintenance, existingEntity);
            existingEntity.FieldMaintenanceDate = adjustedNewDate;

            context.Entry(existingEntity).State = EntityState.Modified;

            try
            {
                if (await databaseService.SaveAll())
                {
                    var resultEntity = await context
                        .FieldMaintenances.Include(f => f.zMaintenanceType)
                        .Include(f => f.zField)
                        .FirstOrDefaultAsync(e =>
                            e.FieldMaintenanceGuid == updateFieldMaintenance.FieldMaintenanceGuid
                        );

                    serviceResponse.Data = mapper.Map<GetFieldMaintenanceDto>(resultEntity);
                    return serviceResponse;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.ToString();
                serviceResponse.Success = false;
                return serviceResponse;
            }
            return serviceResponse;
        }
        #endregion

        #region Production Record
        public async Task<ServiceResponse<GetProductionRecordDto>> AddProductionRecord(
            AddProductionRecordDto newProductionRecord
        )
        {
            var serviceResponse = new ServiceResponse<GetProductionRecordDto>();
            var currentUserId = httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.UserData)
                ?.Value;

            // Add 8 hours and ensure UTC
            var adjustedDate = DateTime.SpecifyKind(
                newProductionRecord.DateOfProduction.AddHours(8),
                DateTimeKind.Utc
            );
            
            // Create the start and end of month in UTC
            var startOfMonth = new DateTime(adjustedDate.Year, adjustedDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1);

            var existingRecord = await context.ProductionRecords
                .AnyAsync(p => 
                    p.zFieldId == newProductionRecord.zFieldId &&
                    p.DateOfProduction >= startOfMonth &&
                    p.DateOfProduction < endOfMonth
                );

            if (existingRecord)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"A production record already exists for {adjustedDate:MMMM yyyy} for this field.";
                return serviceResponse;
            }

            // Map DTO to entity
            var productionRecordEntity = mapper.Map<ProductionRecord>(newProductionRecord);

            // Set the date with the 8-hour adjustment and UTC kind
            productionRecordEntity.DateOfProduction = adjustedDate;
            productionRecordEntity.CreatedBy = currentUserId;

            databaseService.add(productionRecordEntity);

            if (await databaseService.SaveAll())
            {
                var resultEntity = await context
                    .ProductionRecords.Include(p => p.zField)
                    .FirstOrDefaultAsync(e =>
                        e.ProductionRecordGuid == productionRecordEntity.ProductionRecordGuid
                    );

                serviceResponse.Data = mapper.Map<GetProductionRecordDto>(resultEntity);
                return serviceResponse;
            }

            serviceResponse.Success = false;
            serviceResponse.Message = "Error in Adding Data";
            return serviceResponse;
        }

        public async Task<bool> CheckProductionRecordById(Guid id)
        {
            return await context.ProductionRecords.AnyAsync(e => e.ProductionRecordGuid == id);
        }

        public async Task<ServiceResponse<List<GetProductionRecordDto>>> DeleteProductionRecord(
            Guid id
        )
        {
            var serviceResponse = new ServiceResponse<List<GetProductionRecordDto>>();
            try
            {
                var productionRecord = await context.ProductionRecords.FirstOrDefaultAsync(c =>
                    c.ProductionRecordGuid == id
                );
                if (productionRecord is null)
                    throw new Exception($"Production Record with ID '{id}' not found.");

                context.ProductionRecords.Remove(productionRecord);
                await context.SaveChangesAsync();

                var productionRecordEntities = await context
                    .ProductionRecords.Include(p => p.zField)
                    .ToListAsync();

                serviceResponse.Data = mapper.Map<List<GetProductionRecordDto>>(
                    productionRecordEntities
                );
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.Message;
                serviceResponse.Success = false;
            }

            return serviceResponse;
        }

        public async Task<
            ServiceResponse<PagedList<GetProductionRecordDto>>
        > GetAllProductionRecords(Pagination pagination)
        {
            // Null check for parameter
            pagination ??= new Pagination();

            var query = context.ProductionRecords.Include(p => p.zField).AsQueryable();

            var projectedQuery = query.ProjectTo<GetProductionRecordDto>(
                mapper.ConfigurationProvider
            );

            var pagedData = await Task.FromResult(
                PagedList<GetProductionRecordDto>.ToPagedList(
                    projectedQuery,
                    pagination.PageNumber,
                    pagination.PageSize
                )
            );

            var serviceResponse = new ServiceResponse<PagedList<GetProductionRecordDto>>
            {
                Data = pagedData,
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetProductionRecordDto>> GetProductionRecordById(Guid id)
        {
            var serviceResponse = new ServiceResponse<GetProductionRecordDto>();
            var productionRecord = await context
                .ProductionRecords.Include(p => p.zField)
                .FirstOrDefaultAsync(e => e.ProductionRecordGuid == id);

            if (productionRecord is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Production Record not found!";
                return serviceResponse;
            }

            serviceResponse.Data = mapper.Map<GetProductionRecordDto>(productionRecord);
            return serviceResponse;
        }

        public async Task<
            ServiceResponse<PagedList<GetProductionRecordDto>>
        > GetProductionRecordsPagination(
            Pagination pagination,
            string search = null,
            ProductionRecordFilter productionRecordFilter = null
        )
        {
            // Null checks for parameters
            pagination ??= new Pagination();
            productionRecordFilter ??= new ProductionRecordFilter();

            // Ensure required fields have default values
            pagination.SortColumn ??= "DateOfProduction"; // Default sort column
            pagination.SortDirection ??= "desc"; // Default sort direction

            // Start with a base query
            var query = context.ProductionRecords.Include(p => p.zField).AsQueryable();

            // Apply filters
            if (productionRecordFilter.zFieldId.HasValue)
                query = query.Where(e => e.zFieldId == productionRecordFilter.zFieldId.Value);

            // Apply Year of Extraction filter
            if (productionRecordFilter.Year.HasValue)
            {
                int year = productionRecordFilter.Year.Value;
                var startDate = new DateTime(year, 1, 1).ToUniversalTime();
                var endDate = new DateTime(year + 1, 1, 1).ToUniversalTime();
                query = query.Where(e =>
                    e.DateOfProduction >= startDate && e.DateOfProduction < endDate
                );
            }
            else
            {
                // Apply date range filters if year is not specified
                if (productionRecordFilter.StartDate.HasValue)
                {
                    var startDate = productionRecordFilter.StartDate.Value.ToUniversalTime();
                    query = query.Where(e => e.DateOfProduction >= startDate);
                }

                if (productionRecordFilter.EndDate.HasValue)
                {
                    var endDate = productionRecordFilter.EndDate.Value.ToUniversalTime().AddDays(1);
                    query = query.Where(e => e.DateOfProduction < endDate);
                }
            }

            // Apply Production Rate filters
            if (productionRecordFilter.MinProductionRate.HasValue)
                query = query.Where(e =>
                    e.ProductionRate >= productionRecordFilter.MinProductionRate.Value
                );

            if (productionRecordFilter.MaxProductionRate.HasValue)
                query = query.Where(e =>
                    e.ProductionRate <= productionRecordFilter.MaxProductionRate.Value
                );

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                // Could search by field name
                query = query.Where(x => x.zField.Name != null && x.zField.Name.Contains(search));
            }

            // Apply ordering
            query = ApplySorting(query, pagination.SortColumn, pagination.SortDirection);

            // Project to DTO and apply pagination
            var projectedQuery = query.ProjectTo<GetProductionRecordDto>(
                mapper.ConfigurationProvider
            );

            var pagedData = await Task.FromResult(
                PagedList<GetProductionRecordDto>.ToPagedList(
                    projectedQuery,
                    pagination.PageNumber,
                    pagination.PageSize
                )
            );

            return new ServiceResponse<PagedList<GetProductionRecordDto>>
            {
                Data = pagedData,
                Success = true,
            };
        }

        public async Task<ServiceResponse<GetProductionRecordDto>> UpdateProductionRecord(
            UpdateProductionRecordDto updateProductionRecord
        )
        {
            var serviceResponse = new ServiceResponse<GetProductionRecordDto>();
            var currentUserId = httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.UserData)
                ?.Value;

            var existingEntity = await context.ProductionRecords.FindAsync(
                updateProductionRecord.ProductionRecordGuid
            );

            if (existingEntity == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Production Record not found!";
                return serviceResponse;
            }

            // Add 8 hours and ensure UTC for the new date
            var adjustedNewDate = DateTime.SpecifyKind(
                updateProductionRecord.DateOfProduction.AddHours(8),
                DateTimeKind.Utc
            );

            // Check if the month is being changed
            if (existingEntity.DateOfProduction.Year != adjustedNewDate.Year ||
                existingEntity.DateOfProduction.Month != adjustedNewDate.Month)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Cannot change the month of an existing production record.";
                return serviceResponse;
            }

            existingEntity.UpdatedBy = currentUserId;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            // Map DTO to entity, preserving the ID and audit fields
            mapper.Map(updateProductionRecord, existingEntity);
            existingEntity.DateOfProduction = adjustedNewDate;

            context.Entry(existingEntity).State = EntityState.Modified;

            try
            {
                if (await databaseService.SaveAll())
                {
                    var resultEntity = await context
                        .ProductionRecords.Include(p => p.zField)
                        .FirstOrDefaultAsync(e =>
                            e.ProductionRecordGuid == updateProductionRecord.ProductionRecordGuid
                        );

                    serviceResponse.Data = mapper.Map<GetProductionRecordDto>(resultEntity);
                    return serviceResponse;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.ToString();
                serviceResponse.Success = false;
                return serviceResponse;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> ImportProductionRecordsFromCsv(IFormFile csvFile)
        {
            var serviceResponse = new ServiceResponse<int>();

            try
            {
                if (csvFile == null || csvFile.Length == 0)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No file was uploaded";
                    return serviceResponse;
                }

                if (
                    !Path.GetExtension(csvFile.FileName)
                        .Equals(".csv", StringComparison.OrdinalIgnoreCase)
                )
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Only CSV files are supported";
                    return serviceResponse;
                }

                int importedCount = 0;
                List<string> errors = new List<string>();

                // Setup CSV configuration
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    DetectDelimiter = true,
                    BadDataFound = null,
                    TrimOptions = TrimOptions.Trim,
                };

                List<ProductionRecordCsvImportDto> records =
                    new List<ProductionRecordCsvImportDto>();

                using (var reader = new StreamReader(csvFile.OpenReadStream()))
                using (var csv = new CsvReader(reader, config))
                {
                    // Register the class map
                    csv.Context.RegisterClassMap<ProductionRecordCsvImportMap>();
                    records = csv.GetRecords<ProductionRecordCsvImportDto>().ToList();
                }

                var currentUserId = httpContextAccessor
                    .HttpContext?.User?.FindFirst(ClaimTypes.UserData)
                    ?.Value;

                foreach (var record in records)
                {
                    try
                    {
                        // Map DTO to entity
                        var productionRecord = mapper.Map<ProductionRecord>(record);

                        // Set additional properties
                        productionRecord.ProductionRecordGuid = Guid.NewGuid();
                        productionRecord.DateOfProduction =
                            record.DateOfProduction.ToUniversalTime();
                        productionRecord.CreatedBy = currentUserId;

                        databaseService.add(productionRecord);
                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error importing record: {ex.Message}");
                    }
                }

                await databaseService.SaveAll();

                serviceResponse.Data = importedCount;
                serviceResponse.Message =
                    $"Successfully imported {importedCount} production records."
                    + (errors.Any() ? $" Errors: {string.Join("; ", errors)}" : "");

                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error importing CSV: {ex.Message}";
                return serviceResponse;
            }
        }

        // Define a ClassMap for CSV import
        private class ProductionRecordCsvImportMap : ClassMap<ProductionRecordCsvImportDto>
        {
            public ProductionRecordCsvImportMap()
            {
                Map(m => m.DateOfProduction);
                Map(m => m.ProductionOfCost);
                Map(m => m.ProductionRate);
                Map(m => m.zFieldId);
            }
        }

        public async Task<
            ServiceResponse<List<BackEndGasApp.Dtos.ProductionRecord.YearDisabledMonthsDto>>
        > GetDisabledMonths(int? fieldId = null)
        {
            var serviceResponse =
                new ServiceResponse<
                    List<BackEndGasApp.Dtos.ProductionRecord.YearDisabledMonthsDto>
                >();

            try
            {
                // Query to get all production records, optionally filtered by field
                var query = context.ProductionRecords.AsQueryable();
                if (fieldId.HasValue)
                {
                    query = query.Where(p => p.zFieldId == fieldId.Value);
                }

                // Group by year and month to find months with existing records
                var existingRecords = await query
                    .Select(p => new
                    {
                        Year = p.DateOfProduction.Year,
                        Month = p.DateOfProduction.Month - 1, // Convert to 0-based month index (0-11)
                    })
                    .Distinct()
                    .ToListAsync();

                // Group by year
                var groupedByYear = existingRecords
                    .GroupBy(r => r.Year)
                    .Select(g => new BackEndGasApp.Dtos.ProductionRecord.YearDisabledMonthsDto
                    {
                        Year = g.Key,
                        DisabledMonths = g.Select(x => x.Month).ToList(),
                    })
                    .ToList();

                serviceResponse.Data = groupedByYear;
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error retrieving disabled months: {ex.Message}";
                return serviceResponse;
            }
        }

        public async Task<
            ServiceResponse<List<YearDisabledMonthsDto>>
        > GetFieldMaintenanceDisabledMonths(int? fieldId = null)
        {
            var serviceResponse = new ServiceResponse<List<YearDisabledMonthsDto>>();

            try
            {
                // Query to get all field maintenance records, optionally filtered by field
                var query = context.FieldMaintenances.AsQueryable();
                if (fieldId.HasValue)
                {
                    query = query.Where(f => f.zFieldId == fieldId.Value);
                }

                // Group by year and month to find months with existing records
                var existingRecords = await query
                    .Select(f => new
                    {
                        Year = f.FieldMaintenanceDate.Year,
                        Month = f.FieldMaintenanceDate.Month - 1, // Convert to 0-based month index (0-11)
                    })
                    .Distinct()
                    .ToListAsync();

                // Group by year
                var groupedByYear = existingRecords
                    .GroupBy(r => r.Year)
                    .Select(g => new YearDisabledMonthsDto
                    {
                        Year = g.Key,
                        DisabledMonths = g.Select(x => x.Month).ToList(),
                    })
                    .ToList();

                serviceResponse.Data = groupedByYear;
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error retrieving disabled months: {ex.Message}";
                return serviceResponse;
            }
        }
        #endregion

        #region Helper Methods
        private IQueryable<T> ApplySorting<T>(
            IQueryable<T> query,
            string sortColumn,
            string sortDirection
        )
        {
            // Default case if sort column is not recognized
            if (string.IsNullOrEmpty(sortColumn))
                return query;

            // Get property info for the sort column
            var propertyInfo = typeof(T).GetProperty(
                sortColumn,
                System.Reflection.BindingFlags.IgnoreCase
                    | System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.Instance
            );

            if (propertyInfo == null)
                return query; // If property doesn't exist, return unsorted query

            // Create a generic sort expression
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            // Apply the sort based on direction
            var methodName = sortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), propertyInfo.PropertyType },
                query.Expression,
                Expression.Quote(lambda)
            );

            return query.Provider.CreateQuery<T>(resultExpression);
        }
        #endregion
    }
}
