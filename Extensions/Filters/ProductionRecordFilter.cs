using System;

namespace BackEndGasApp.Extensions.Filters
{
    public class ProductionRecordFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? zFieldId { get; set; }
        public decimal? MinProductionRate { get; set; }
        public decimal? MaxProductionRate { get; set; }
        public int? Year { get; set; }  // Year of Extraction filter
    }
} 