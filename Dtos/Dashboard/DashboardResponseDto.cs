using System;
using System.Collections.Generic;

namespace BackEndGasApp.Dtos.Dashboard
{
    public class DashboardResponseDto
    {
        public int TotalProductionRate { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
        public List<RegionDistributionDto> RegionDistribution { get; set; } = new List<RegionDistributionDto>();
        public List<ProductionRateChartDto> ProductionRateChart { get; set; } = new List<ProductionRateChartDto>();
        public List<MaintenanceCostChartDto> MaintenanceCostChart { get; set; } = new List<MaintenanceCostChartDto>();
        public List<FieldDataDto> FieldData { get; set; } = new List<FieldDataDto>();
    }

    public class RegionDistributionDto
    {
        public string RegionName { get; set; }
        public int FieldCount { get; set; }
    }

    public class ProductionRateChartDto
    {
        public string Period { get; set; }
        public int ProductionRate { get; set; }
    }

    public class MaintenanceCostChartDto
    {
        public string Period { get; set; }
        public decimal Cost { get; set; }
    }

    public class FieldDataDto
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int ProductionRate { get; set; }
        public int ExtractionYear { get; set; }
        public string MaintenanceType { get; set; }
        public decimal MaintenanceCost { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
    }
} 