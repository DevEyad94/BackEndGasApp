using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndGasApp.Models.zsk
{
    public class zRole
    {
        [Key]
        public int zRoleID { get; set; }
        public string Name { get; set; }
    }
}