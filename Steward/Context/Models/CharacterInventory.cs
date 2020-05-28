using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Steward.Context.Models
{
    public class CharacterInventory
    {
        public string PlayerCharacterId { get; set; }
        [Key]
        public string InventoryId { get; set; }
        public string ValkFinderWeaponId { get; set; }
        public string ValkFinderArmourId { get; set; }
        public string ValkFinderItemId { get; set; }
        public int Amount { get; set; }
    }
}
