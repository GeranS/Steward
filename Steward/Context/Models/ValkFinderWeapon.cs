using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Steward.Context.Models
{
	public class ValkFinderWeapon
	{
		[Key]
		public string ValkFinderWeaponId { get; set; }
		[Required]
		public string WeaponName { get; set; }
		public string WeaponDescription { get; set; }
		[Required]
		public bool IsRanged { get; set; }
		[Required]
		public int DamageDieSize { get; set; }
		public int DamageDieAmount { get; set; }
		public int DamageBonus { get; set; }
		public int HitBonus { get; set; }
		public bool IsUnique { get; set; }

		public WeaponTrait WeaponTrait { get; set; }



	}

	public enum WeaponTrait
	{
		Finesse,
		None
	}
}
