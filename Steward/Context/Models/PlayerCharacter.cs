using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Steward.Context.Models
{
	public class PlayerCharacter
	{
		[Key]
		public string CharacterId { get; set; } = Guid.NewGuid().ToString();

		public string CharacterName { get; set; }

		public string Bio { get; set; } = "None.";

		public int STR { get; set; } = 8;
		public int END { get; set; } = 8;
		public int DEX { get; set; } = 8;
		public int PER { get; set; } = 8;
		public int INT { get; set; } = 8;

		public string DefaultMeleeWeaponId { get; set; }
		public ValkFinderWeapon DefaultMeleeWeapon { get; set; }

		public string DefaultRangedWeaponId { get; set; }
		public ValkFinderWeapon DefaultRangedWeapon { get; set; }

		public int InitialAge { get; set; } = new Random().Next(18, 25);
		public int YearOfBirth { get; set; }
		// so it can be assigned "???"
		public string YearOfDeath { get; set; }

		public List<CharacterTrait> CharacterTraits { get; set; }

		public string DiscordUserId { get; set; }
		public DiscordUser DiscordUser { get; set; }

		public string HouseId { get; set; }
		public House House { get; set; }

		public int GetAge(int currentYear)
		{
			var age = currentYear - YearOfBirth;
			return age;
		}

		public bool IsAlive()
		{
			return YearOfDeath == null;
		}
	}

	public enum CharacterAttribute
	{
		STR,
		END,
		DEX,
		PER,
		INT
	}
}
