using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text;

namespace Steward.Context.Models
{ 
	public class House
	{
		[Key]
		public string HouseId { get; set; } = Guid.NewGuid().ToString();

		public string HouseName { get; set; }

		public string HouseDescription { get; set; } = "Empty.";

		public int STR { get; set; } = 0;
		public int DEX { get; set; } = 0;
		public int END { get; set; } = 0;
		public int PER { get; set; } = 0;
		public int INT { get; set; } = 0;

		public int ArmorClassBonus { get; set; } = 0;
		public int HealthPoolBonus { get; set; } = 0;
		public int AbilityPointBonus { get; set; } = 0;

		public string HouseOwnerId { get; set; }
		public PlayerCharacter HouseOwner { get; set; }

		public List<PlayerCharacter> HouseMembers { get; set; }
	}
}
