using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndGasApp.Models
{
    public class RefModel
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; } = "System";
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
