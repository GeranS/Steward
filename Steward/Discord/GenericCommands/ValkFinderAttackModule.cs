﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
	public class ValkFinderAttackModule : ModuleBase<SocketCommandContext>
	{
		private readonly RollService _rollService;
		private readonly StewardContext _stewardContext;

		public ValkFinderAttackModule(StewardContext c, RollService r)
		{
			_rollService = r;
			_stewardContext = c;
		}

		[Command("equip")]
		[RequireActiveCharacter]
		public async Task SetDefaultWeapon(string weaponName)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.DefaultMeleeWeapon)
					.Include(c => c.DefaultRangedWeapon)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

			var valkFinderWeapon =
				_stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == weaponName);

			if (valkFinderWeapon == null)
			{
				await ReplyAsync($"Could not find weapon.");
				return;
			}

			if (valkFinderWeapon.IsRanged)
			{
				activeCharacter.DefaultRangedWeapon = valkFinderWeapon;
			}
			else
			{
				activeCharacter.DefaultMeleeWeapon = valkFinderWeapon;
			}

			_stewardContext.PlayerCharacters.Update(activeCharacter);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync($"{valkFinderWeapon.WeaponName} has been equipped.");
		}

		[Command("melee")]
		[RequireActiveCharacter]
		public async Task AttackWithMeleeWeapon(string attackType = "normal", string weaponName = null)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.DefaultMeleeWeapon)
					.Include(c => c.House)
					.Include(c => c.CharacterTraits)
					.ThenInclude(ct => ct.Trait)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

			ValkFinderWeapon valkFinderWeapon = null;

			valkFinderWeapon = _stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == weaponName) ?? activeCharacter.DefaultMeleeWeapon;

			if (valkFinderWeapon == null)
			{
				await ReplyAsync($"Could not find weapon.");
				return;
			}

			if (valkFinderWeapon.IsRanged)
			{
				await ReplyAsync($"{valkFinderWeapon.WeaponName} is not a melee weapon.");
				return;
			}

			var message = _rollService.RollMeleeAttack(activeCharacter, valkFinderWeapon, attackType);

			await ReplyAsync(embed: message.Build());
		}

		[Command("ranged")]
		[RequireActiveCharacter]
		public async Task AttackWithRangedWeapon(int range, string weaponName = null)
		{
			if (range < 0)
			{
				await ReplyAsync($"Range can't be negative.");
				return;
			}

			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.DefaultRangedWeapon)
					.Include(c => c.House)
					.Include(c => c.CharacterTraits)
					.ThenInclude(ct => ct.Trait)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

			ValkFinderWeapon valkFinderWeapon = null;

			valkFinderWeapon = activeCharacter.DefaultRangedWeapon ?? _stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == weaponName);

			if (valkFinderWeapon == null)
			{
				await ReplyAsync($"Could not find weapon.");
				return;
			}

			if (!valkFinderWeapon.IsRanged)
			{
				await ReplyAsync($"{weaponName} is not a ranged weapon.");
				return;
			}

			var message = _rollService.RollRangedAttack(activeCharacter, valkFinderWeapon, range);

			await ReplyAsync(embed: message.Build());
		}

		[Command("weapons")]
		public async Task WeaponList()
		{
			var embedBuilder = new EmbedBuilder();

			var valkFinderWeapons = _stewardContext.ValkFinderWeapons.ToList();

			var sortedValkFinderWeapons = valkFinderWeapons.OrderBy(v => v.IsRanged).ThenBy(v => v.WeaponName);

			var stringBuilder = new StringBuilder();

			foreach (var weapon in sortedValkFinderWeapons)
			{
				if (weapon.IsRanged && !weapon.IsUnique)
				{
					stringBuilder.AppendLine($"{weapon.WeaponName}: {weapon.DamageDieAmount}d{weapon.DamageDieSize}+{weapon.DamageBonus} ranged");
				}
				else if (!weapon.IsRanged && !weapon.IsUnique)
				{
					stringBuilder.AppendLine($"{weapon.WeaponName}: {weapon.DamageDieAmount}d{weapon.DamageDieSize}+{weapon.DamageBonus} melee");
				}
			}

			embedBuilder.AddField("Weapons", stringBuilder.ToString());

			await ReplyAsync(embed: embedBuilder.Build());
		}

		[Command("add weapon")]
		[RequireStewardPermission]
		public async Task AddWeapon(string name, string rangedOrMelee, string damage, int hitbonus, string weaponTraitString = "None",bool unique = false)
		{
			if (name.Length > 100)
			{
				await ReplyAsync("Weapon name can't be longer than 100 characters.");
				return;
			}

			if (rangedOrMelee != "ranged" && rangedOrMelee != "melee")
			{
				await ReplyAsync("Weapon has to be either melee or ranged.");
				return;
			}
			var damageDieAmount = 0;
			var damageDieSize = 0;
			var damageBonus = 0;

			try
			{
				var pattern = @"(\d+)d(\d+)([+-])(\d+)";
				foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(damage,pattern))
				{
					damageDieAmount = Int32.Parse(m.Groups[1].Value);
					damageDieSize = Int32.Parse(m.Groups[2].Value);
					damageBonus = Int32.Parse(m.Groups[3].Value + m.Groups[4].Value);
				}
			}
			catch
			{
				await ReplyAsync($"{damage}: is not a valid damage die and modifier");
				return;
			}

			if (!Enum.TryParse(weaponTraitString, true, out WeaponTrait weaponTrait))
			{
				await ReplyAsync("Not a valid attribute.");
				return;
			}

			var doesWeaponAlreadyExist =
				_stewardContext.ValkFinderWeapons.SingleOrDefault(vfw => vfw.WeaponName == name);

			if (doesWeaponAlreadyExist != null)
			{
				await ReplyAsync("Weapon already exists.");
				return;
			}

			var newWeapon = new ValkFinderWeapon()
			{
				WeaponName = name,
				DamageDieAmount = damageDieAmount,
				DamageDieSize = damageDieSize,
				DamageBonus = damageBonus,
				HitBonus = hitbonus,
				IsRanged = rangedOrMelee == "ranged",
				IsUnique = unique,
				WeaponTrait = weaponTrait
			};

			_stewardContext.ValkFinderWeapons.Add(newWeapon);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Weapon added.");
		}
	}
}