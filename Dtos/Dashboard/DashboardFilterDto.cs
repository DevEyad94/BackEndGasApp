using System;
using System.Collections.Generic;

namespace BackEndGasApp.Dtos.Dashboard
{
    public class DashboardFilterDto
    {
        public int? MinProductionRate { get; set; }
        public int? MaxProductionRate { get; set; }
        public int? ExtractionYear { get; set; }
        public int? FromYear { get; set; }
        public int? ToYear { get; set; }
        public int? MaintenanceTypeId { get; set; }
        public decimal? MinCost { get; set; }
        public decimal? MaxCost { get; set; }
        public int? FieldId { get; set; }
    }
} 