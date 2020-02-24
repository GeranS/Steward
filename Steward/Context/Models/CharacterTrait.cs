using System;
using System.Collections.Generic;
using System.Text;

namespace Steward.Context.Models
{
	public class CharacterTrait
	{
		public string PlayerCharacterId { get; set; }
		public PlayerCharacter PlayerCharacter{ get; set; }

		public string TraitId { get; set; }
		public Trait Trait { get; set; }
	}
}
