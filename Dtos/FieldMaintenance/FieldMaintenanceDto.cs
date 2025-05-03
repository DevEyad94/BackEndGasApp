using System;
using BackEndGasApp.Models.zsk;

namespace BackEndGasApp.Dtos.FieldMaintenance
{
    // DTO for getting field maintenance data
    public class GetFieldMaintenanceDto
    {
        public Guid FieldMaintenanceGuid { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public DateTime FieldMaintenanceDate { get; set; }
        public int zMaintenanceTypeId { get; set; }
        public string MaintenanceTypeName { get; set; } // Friendly name from zMaintenanceType
        public int zFieldId { get; set; }
        public string FieldName { get; set; } // Friendly name from zField
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    // DTO for adding new field maintenance
    public class AddFieldMaintenanceDto
    {
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public DateTime FieldMaintenanceDate { get; set; }
        public int zMaintenanceTypeId { get; set; }
        public int zFieldId { get; set; }
    }

    // DTO for updating field maintenance
    public class UpdateFieldMaintenanceDto
    {
        public Guid FieldMaintenanceGuid { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public DateTime FieldMaintenanceDate { get; set; }
        public int zMaintenanceTypeId { get; set; }
        public int zFieldId { get; set; }
    }
}
