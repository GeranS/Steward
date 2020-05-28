using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Steward.Context.Models
{
	public class CharacterDeathTimer
	{
		[Key]
		public string CharacterDeathTimerId { get; set; } = Guid.NewGuid().ToString();

		public string PlayerCharacterId { get; set; }
		public PlayerCharacter PlayerCharacter { get; set; }

		public int YearOfDeath { get; set; }

		[Required]
		public DateTime DeathTime { get; set; }
	}
}
