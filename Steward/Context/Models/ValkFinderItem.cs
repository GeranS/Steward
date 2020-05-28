using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Steward.Context.Models
{
    public class ValkFinderItem
    {
        [Key]
        public string ValkFinderItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
    }
}
