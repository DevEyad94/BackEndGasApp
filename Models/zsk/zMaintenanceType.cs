using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndGasApp.Models.zsk
{
    public class zMaintenanceType
    {
        [Key]
        public int zMaintenanceTypeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
