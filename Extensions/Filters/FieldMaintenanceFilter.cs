using System;

namespace BackEndGasApp.Extensions.Filters
{
    public class FieldMaintenanceFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? zMaintenanceTypeId { get; set; }
        public int? zFieldId { get; set; }
        public decimal? MinCost { get; set; }
        public decimal? MaxCost { get; set; }
    }
}
