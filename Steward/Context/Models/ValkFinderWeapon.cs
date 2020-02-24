using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Steward.Context.Models
{
	public class ValkFinderWeapon
	{
		[Key]
		public string WeaponName { get; set; }
		[Required]
		public bool IsRanged { get; set; }
		[Required]
		public int DieSize { get; set; }
		[Required]
		public CharacterAttribute DamageModifier { get; set; }
	}
}
