using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Models.zsk;

namespace BackEndGasApp.Models
{
    public class ProductionRecord : RefModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductionRecordGuid { get; set; }
        public DateTime DateOfProduction { get; set; }
        public decimal ProductionOfCost { get; set; }
        public int ProductionRate { get; set; }
        public zField zField { get; set; }
        public int zFieldId { get; set; }
    }
}
