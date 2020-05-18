using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.WebSocket;
using Steward.Context;
using Steward.Context.Models;

namespace Steward.Services
{
	public class RollService
	{

		private readonly StewardContext _stewardContext;

		public RollService(StewardContext stewardContext)
		{
			_stewardContext = stewardContext;
		}

		public EmbedBuilder RollMeleeAttack(PlayerCharacter character, ValkFinderWeapon weapon, string attackType)
		{

			var attackTypeHitBonus = 0;
			var attackTypeDamageBonus = 0;

			switch (attackType)
			{
				case "heavy":
					attackTypeHitBonus = 1;
					attackTypeDamageBonus = 1;
					break;
				case "light":
					attackTypeHitBonus = -1;
					attackTypeDamageBonus = -1;
					break;
				case "normal":
					break;
				default:
					var embedBuilderError = new EmbedBuilder().WithColor(Color.Purple);
					embedBuilderError.AddField("Error", "Unknown attack type");
					return embedBuilderError;
			}

			var rnd = new Random();

			var strMod = GetStatAsModifier(CharacterAttribute.STR, character);
			var dexMod = GetStatAsModifier(CharacterAttribute.DEX, character);

			var attackRoll = rnd.Next(1, 20) + strMod + dexMod + attackTypeHitBonus;

			if (attackRoll < 0)
			{
				attackRoll = 0;
			}

			var damageRollBonus = 0; //GetStatAsModifier(weapon.DamageModifier, character); -- Leaving this commented out so this can be re-implemented if we ever need to do so.
			var damageRoll = rnd.Next(1, weapon.DieSize) + damageRollBonus + attackTypeDamageBonus;

			if (damageRoll < 0)
			{
				damageRoll = 0;
			}

			var attackRollString = $"1d20 + {strMod + dexMod} = {attackRoll}";
			if (attackTypeHitBonus != 0)
			{
				attackRollString = $"1d20 + {strMod + dexMod} + {attackTypeHitBonus} from attack type = {attackRoll}";
			}
			
			var damageRollString = $"1d{weapon.DieSize} + {damageRollBonus} = {damageRoll}";
			if (attackTypeDamageBonus != 0)
			{
				damageRollString = $"1d{weapon.DieSize} + {damageRollBonus} + {attackTypeDamageBonus} from attack type = {damageRoll}";
			}

			var embedBuilder = new EmbedBuilder().WithColor(Color.Purple).WithTitle($"Melee: {weapon.WeaponName} ({attackType}) by {character.CharacterName}");

			embedBuilder.AddField("Attack Roll", attackRollString);
			embedBuilder.AddField("Damage Roll", damageRollString);

			return embedBuilder;
		}

		public EmbedBuilder RollRangedAttack(PlayerCharacter character, ValkFinderWeapon weapon, int range)
		{
			var rnd = new Random();

			var perMod = GetStatAsModifier(CharacterAttribute.PER, character);
			var dexMod = GetStatAsModifier(CharacterAttribute.DEX, character);

			var rangePenalty = -(range / 2);

			var attackRoll = rnd.Next(1, 20) + perMod + dexMod + rangePenalty;

			if (attackRoll < 0)
			{
				attackRoll = 0;
			}

			var damageRollBonus = 0; //GetStatAsModifier(weapon.DamageModifier, character); -- ―〃―
			var damageRoll = rnd.Next(1, weapon.DieSize) + damageRollBonus;

			if (damageRoll < 0)
			{
				damageRoll = 0;
			}
			var attackRollString = $"1d20 + {perMod + dexMod} - {rangePenalty*-1} = {attackRoll}";
			var damageRollString = $"1d{weapon.DieSize} + {damageRollBonus} = {damageRoll}";

			var embedBuilder = new EmbedBuilder().WithColor(Color.Purple).WithTitle($"Ranged: {weapon.WeaponName}");

			embedBuilder.AddField("Attack Roll", attackRollString);
			embedBuilder.AddField("Damage Roll", damageRollString);

			return embedBuilder;
		}

		public string RollPlayerStat(CharacterAttribute attribute, PlayerCharacter character, int dice)
		{
			var mod = GetStatAsModifier(attribute, character);

			var rnd = new Random();

			var roll = rnd.Next(1, dice) + mod;

			if (mod >= 0)
			{
				return $"d{dice}+{mod} = [{roll}]";
			}
			else
			{
				return $"d{dice}{mod} = [{roll}]";
			}
		}

		/// <summary>
		/// Calculates the modifier of a certain stat, taking bonuses from Traits into account.
		/// </summary>
		/// <param name="attribute"></param>
		/// <param name="character"></param>
		/// <returns></returns>
		public int GetStatAsModifier(CharacterAttribute attribute, PlayerCharacter character)
		{
			return attribute switch
			{
				CharacterAttribute.STR => StatToMod(CalculateStat(CharacterAttribute.STR, character)),
				CharacterAttribute.END => StatToMod(CalculateStat(CharacterAttribute.END, character)),
				CharacterAttribute.DEX => StatToMod(CalculateStat(CharacterAttribute.DEX, character)),
				CharacterAttribute.PER => StatToMod(CalculateStat(CharacterAttribute.PER, character)),
				CharacterAttribute.INT => StatToMod(CalculateStat(CharacterAttribute.INT, character)),
				_ => 0,
			};
		}

		public EmbedBuilder RollPlayerDodge(PlayerCharacter character)
		{
			var rnd = new Random();
			
			var perMod = GetStatAsModifier(CharacterAttribute.PER, character);
			var dexMod = GetStatAsModifier(CharacterAttribute.DEX, character);

			var dodgeRoll = rnd.Next(1, 20) + perMod + dexMod;
			var dodgeRollString = $"1d20+{perMod} + {dexMod} = {dodgeRoll}";
			
			var embedBuilder = new EmbedBuilder().WithColor(Color.DarkPurple).WithTitle($"Dodge");

			embedBuilder.AddField("Dodge Roll", dodgeRollString);

			return embedBuilder;
		}

		public int CalculateStat(CharacterAttribute attribute, PlayerCharacter character)
		{
			var totalBonus = 0;
			var baseStat = 0;

			switch (attribute)
			{
				case CharacterAttribute.STR:
					foreach (var trait in character.CharacterTraits.Where(ct => ct.Trait.STR != 0).Select(ct => ct.Trait))
					{
						totalBonus += trait.STR;
					}

					if (character.House != null)
					{
						totalBonus += character.House.STR;
					}
					
					baseStat = character.STR;
					break;
				case CharacterAttribute.DEX:
					foreach (var trait in character.CharacterTraits.Where(ct => ct.Trait.DEX != 0).Select(ct => ct.Trait))
					{
						totalBonus += trait.DEX;
					}

					if (character.House != null)
					{
						totalBonus += character.House.DEX;
					}

					baseStat = character.DEX;
					break;
				case CharacterAttribute.END:
					foreach (var trait in character.CharacterTraits.Where(ct => ct.Trait.END != 0).Select(ct => ct.Trait))
					{
						totalBonus += trait.END;
					}

					if (character.House != null)
					{
						totalBonus += character.House.END;
					}

					baseStat = character.END;
					break;
				case CharacterAttribute.PER:
					foreach (var trait in character.CharacterTraits.Where(ct => ct.Trait.PER != 0).Select(ct => ct.Trait))
					{
						totalBonus += trait.PER;
					}

					if (character.House != null)
					{
						totalBonus += character.House.PER;
					}

					baseStat = character.PER;
					break;
				case CharacterAttribute.INT:
					foreach (var trait in character.CharacterTraits.Where(ct => ct.Trait.INT != 0).Select(ct => ct.Trait))
					{
						totalBonus += trait.INT;
					}

					if (character.House != null)
					{
						totalBonus += character.House.INT;
					}

					baseStat = character.INT;
					break;
			}

			var endResult = baseStat + totalBonus;

			return endResult;
		}

		private static int StatToMod(int stat)
		{
			return stat switch
			{
				1 => -5,
				2 => -4,
				3 => -4,
				4 => -3,
				5 => -3,
				6 => -2,
				7 => -2,
				8 => -1,
				9 => -1,
				10 => 0,
				11 => 0,
				12 => 1,
				13 => 1,
				14 => 2,
				15 => 2,
				16 => 3,
				17 => 3,
				18 => 4,
				19 => 4,
				20 => 5,
				_ => 0
			};
		}
	}
}
