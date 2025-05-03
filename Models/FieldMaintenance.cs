using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Models.zsk;

namespace BackEndGasApp.Models
{
    public class FieldMaintenance : RefModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FieldMaintenanceGuid { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public DateTime FieldMaintenanceDate { get; set; }
        public zMaintenanceType zMaintenanceType { get; set; }
        public int zMaintenanceTypeId { get; set; }
        public zField zField { get; set; }
        public int zFieldId { get; set; }
    }
}
