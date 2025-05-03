using System;
using System.Collections.Generic;

namespace BackEndGasApp.Dtos.Dashboard
{
    public class FieldDashboardDto
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal TotalProductionRate { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
        public List<ProductionRateChartDto> ProductionRateChart { get; set; }
        public List<MaintenanceCostChartDto> MaintenanceCostChart { get; set; }
    }
} 