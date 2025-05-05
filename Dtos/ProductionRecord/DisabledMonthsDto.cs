using System.Collections.Generic;

namespace BackEndGasApp.Dtos.ProductionRecord
{
    // DTO for returning years and their disabled months
    public class YearDisabledMonthsDto
    {
        public int Year { get; set; }
        public List<int> DisabledMonths { get; set; }
    }

    // Response DTO for the API
    public class DisabledMonthsResponseDto
    {
        public List<YearDisabledMonthsDto> Data { get; set; }
    }
} 