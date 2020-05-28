using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Steward.Context.Models
{
	public class Trait
	{
		[Key]
		public string Id { get; set; } = Guid.NewGuid().ToString();

		public int STR { get; set; } = 0;
		public int END { get; set; } = 0;
		public int DEX { get; set; } = 0;
		public int PER { get; set; } = 0;
		public int INT { get; set; } = 0;

		public int ArmorClassBonus { get; set; } = 0;
		public int HealthPoolBonus { get; set; } = 0;
		public int AbilityPointBonus { get; set; } = 0;

		[Required]
		public string Description { get; set; }

		public bool IsSecret { get; set; }
		public bool IsEducation { get; set; }

		public List<CharacterTrait> PlayerTraits { get; set; }
	}
}