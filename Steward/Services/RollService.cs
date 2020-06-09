using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    attackTypeHitBonus = 2;
                    attackTypeDamageBonus = 1;
                    break;
                case "light":
                    attackTypeHitBonus = -4;
                    attackTypeDamageBonus = -2;
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
            var dexMod = 0; //GetStatAsModifier(CharacterAttribute.DEX, character); -- ditto as below

			if (weapon.WeaponTrait == WeaponTrait.Finesse)
			{
				strMod = 0;
				dexMod = GetStatAsModifier(CharacterAttribute.DEX, character); 
			}

            var attackRollRaw = rnd.Next(1, 21);

			var attackRoll = attackRollRaw + strMod + dexMod + attackTypeHitBonus + weapon.HitBonus;

            if (attackRoll < 0)
            {
                attackRoll = 0;
            }

			var crit = attackRollRaw == 20; //are we crittin'?

			var damageRollsRaw = new List<int>();
			for(var i = 0; i < weapon.DamageDieAmount; i++)
			{
				damageRollsRaw.Add(rnd.Next(1, weapon.DamageDieSize + 1));
			}

			var critRolls = new List<int>();
			if (crit)
			{
				for (var i = 0; i < weapon.DamageDieAmount; i++)
				{
					critRolls.Add(rnd.Next(1, weapon.DamageDieSize + 1));
				}
			}
			strMod = GetStatAsModifier(CharacterAttribute.STR, character);
			dexMod = GetStatAsModifier(CharacterAttribute.DEX, character);
			var damage = damageRollsRaw.Sum() + weapon.DamageBonus + attackTypeDamageBonus +critRolls.Sum() +strMod +dexMod;

			//TODO: Clean this up into something modular, redefining the entire string each time just isn't maintainable.
			//Done: cleaned up into something modular
            var attackRollString = $"(1d20 = {attackRollRaw}) + {strMod + dexMod}";
			if (attackTypeHitBonus != 0)
			{
				attackRollString += $" + {attackTypeHitBonus} from attack type";
			}
			if (weapon.HitBonus != 0)
			{
				attackRollString += $" + {weapon.HitBonus} from weapon";
			}
			attackRollString += $" = {attackRoll}";

			if (crit)
			{
				attackRollString += " **!Critical Hit!**";
			}
            
			var damageRollsRawString = string.Join(" + ",damageRollsRaw);
			var critRollsString = string.Join(" + ", critRolls);
			var damageRollString = $"({weapon.DamageDieAmount}d{weapon.DamageDieSize} = {damageRollsRawString})";
			if (crit)
			{
				damageRollString += $" + ({weapon.DamageDieAmount}d{weapon.DamageDieSize} = {critRollsString}) from crit";
			}

			if (attackTypeDamageBonus!= 0)
			{
				damageRollString += $" + {attackTypeDamageBonus} from attack type";
			}

			if (weapon.DamageBonus != 0)
			{
				damageRollString += $" + {weapon.DamageBonus} from weapon";
			}
			damageRollString += $" + {strMod}(STR) + {dexMod}(DEX) = {damage}";

			var embedBuilder = new EmbedBuilder().WithColor(Color.Purple).WithTitle($"Melee: {weapon.WeaponName} ({attackType}) by {character.CharacterName}");

            embedBuilder.AddField("Attack Roll", attackRollString);
            embedBuilder.AddField("Damage Roll", damageRollString);

            return embedBuilder;
        }

        public EmbedBuilder RollRangedAttack(PlayerCharacter character, ValkFinderWeapon weapon, int range = 0)
		{
			var rnd = new Random();

			var perMod = GetStatAsModifier(CharacterAttribute.PER, character);
			var dexMod = GetStatAsModifier(CharacterAttribute.DEX, character);

			var rangePenalty = 0; //-(range / 2);

            var attackRollRaw = rnd.Next(1, 21);
			var attackRoll = attackRollRaw + perMod + dexMod + rangePenalty + weapon.HitBonus;
            var crit = attackRollRaw == 20;

			/*if (attackRoll < 0)
			{
				attackRoll = 0;
			}

			var damageRollBonus = 0; //GetStatAsModifier(weapon.DamageModifier, character); -- ―〃―
            var rawDamageRoll = rnd.Next(1, weapon.DamageDieSize + 1);
			var damageRoll =  rawDamageRoll + damageRollBonus;

            if (crit)
                damageRoll += rawDamageRoll;

            if (damageRoll < 0)
			{
				damageRoll = 0;
			}*/

			var damageRollsRaw = new List<int>();
			for (int i = 0; i < weapon.DamageDieAmount; i++)
			{
				damageRollsRaw.Add(rnd.Next(1, weapon.DamageDieSize + 1));
			}

			var damage = damageRollsRaw.Sum() + weapon.DamageBonus + dexMod;
			if (crit)
			{
				damage = damageRollsRaw.Sum() * 2 + weapon.DamageBonus;
			}

			var attackRollString = $"(1d20 = {attackRollRaw}) + {perMod + dexMod}";
			if (weapon.HitBonus != 0)
			{
				attackRollString += $" + {weapon.HitBonus} from weapon";
			}
			attackRollString += $" = {attackRoll}";

			if (crit)
			{
				attackRollString += " **!Critical Hit!**";
			}

			var damageRollsRawString = string.Join(" + ", damageRollsRaw);
			var damageRollString = $"({weapon.DamageDieAmount}d{weapon.DamageDieSize} = {damageRollsRawString})";
			if (crit)
			{

				damageRollString += $" * 2 from crit";
			}

			if (weapon.DamageBonus != 0)
			{
				damageRollString += $" + {weapon.DamageBonus} from weapon";
			}
			damageRollString += $" + {dexMod}(DEX) = {damage}";

			var embedBuilder = new EmbedBuilder().WithColor(Color.Purple).WithTitle($"Ranged: {weapon.WeaponName}");

			embedBuilder.AddField("Attack Roll", attackRollString);
			embedBuilder.AddField("Damage Roll", damageRollString);

			return embedBuilder;
		}

		public string RollPlayerStat(CharacterAttribute attribute, PlayerCharacter character, int dice)
		{
			var mod = GetStatAsModifier(attribute, character);

			var rnd = new Random();

			var roll = rnd.Next(1, dice + 1) + mod;

			if (mod >= 0)
			{
				return $"d{dice} + {mod} = [{roll}]";
			}
			else
			{
				return $"d{dice} {mod} = [{roll}]";
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

			var dodgeRoll = rnd.Next(1, 21) + perMod + dexMod;
			var dodgeRollString = $"1d20 + {perMod} + {dexMod} = {dodgeRoll}";
			
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
					if (character.EquippedArmour!= null)
					{
						totalBonus -= character.EquippedArmour.DexCost;
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

			if (endResult < 1)
			{
				endResult = 1;
			}

			return endResult;
		}

		private static int StatToMod(int stat)
		{
			return Convert.ToInt32(Math.Floor((Convert.ToDouble(stat) - 10) / 2));
		}
	}
}
