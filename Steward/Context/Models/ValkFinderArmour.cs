using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Steward.Context.Models
{
    public class ValkFinderArmour
    {
        [Key]
        public string ValkFinderArmourId { get; set; }
        public string ArmourName { get; set; }
        public int ArmourClassBonus { get; set; }
        public int DexCost { get; set; }
        public bool IsUnique { get; set; }
    }
}
