using System;
using BackEndGasApp.Models.zsk;

namespace BackEndGasApp.Dtos.ProductionRecord
{
    // DTO for getting production record data
    public class GetProductionRecordDto
    {
        public Guid ProductionRecordGuid { get; set; }
        public DateTime DateOfProduction { get; set; }
        public decimal ProductionOfCost { get; set; }
        public int ProductionRate { get; set; }
        public int zFieldId { get; set; }
        public string FieldName { get; set; } // Friendly name from zField
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    // DTO for adding new production record
    public class AddProductionRecordDto
    {
        public DateTime DateOfProduction { get; set; }
        public decimal ProductionOfCost { get; set; }
        public int ProductionRate { get; set; }
        public int zFieldId { get; set; }
    }

    // DTO for updating production record
    public class UpdateProductionRecordDto
    {
        public Guid ProductionRecordGuid { get; set; }
        public DateTime DateOfProduction { get; set; }
        public decimal ProductionOfCost { get; set; }
        public int ProductionRate { get; set; }
        public int zFieldId { get; set; }
    }

    // DTO for CSV import
    public class ProductionRecordCsvImportDto
    {
        public DateTime DateOfProduction { get; set; }
        public decimal ProductionOfCost { get; set; }
        public int ProductionRate { get; set; }
        public int zFieldId { get; set; }
    }
} 