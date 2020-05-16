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

		[Command("equip melee")]
		public async Task SetDefaultMeleeWeapon(string weaponName)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.DefaultMeleeWeapon)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

			if (activeCharacter == null || !activeCharacter.IsAlive())
			{
				await ReplyAsync($"You don't have a living character.");
				return;
			}

			var valkFinderWeapon =
				_stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == weaponName);

			if (valkFinderWeapon == null)
			{
				await ReplyAsync($"Could not find weapon.");
				return;
			}

			if (valkFinderWeapon.IsRanged)
			{
				await ReplyAsync($"{weaponName} is not a melee weapon.");
				return;
			}

			activeCharacter.DefaultMeleeWeapon = valkFinderWeapon;

			_stewardContext.PlayerCharacters.Update(activeCharacter);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync($"{valkFinderWeapon.WeaponName} has been set as default melee weapon.");
		}

		[Command("melee")]
		public async Task AttackWithMeleeWeapon(string attackType = "normal", string weaponName = null)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.DefaultMeleeWeapon)
					.Include(c => c.House)
					.Include(c => c.CharacterTraits)
					.ThenInclude(ct => ct.Trait)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

			if (activeCharacter == null || !activeCharacter.IsAlive())
			{
				await ReplyAsync($"You don't have a living character.");
				return;
			}

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

		[Command("equip ranged")]
		public async Task SetDefaultRangedWeapon(string weaponName)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.DefaultRangedWeapon)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

			if (activeCharacter == null || !activeCharacter.IsAlive())
			{
				await ReplyAsync($"You don't have a living character.");
				return;
			}

			var valkFinderWeapon =
				_stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == weaponName);

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

			activeCharacter.DefaultRangedWeapon = valkFinderWeapon;

			_stewardContext.PlayerCharacters.Update(activeCharacter);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync($"{valkFinderWeapon.WeaponName} has been set as default ranged weapon.");
		}

		[Command("ranged")]
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

			if (activeCharacter == null || !activeCharacter.IsAlive())
			{
				await ReplyAsync($"You don't have a living character.");
				return;
			}

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

			foreach (var weapon in sortedValkFinderWeapons)
			{
				if (weapon.IsRanged)
				{
					embedBuilder.AddField(weapon.WeaponName, $"1d{weapon.DieSize} ranged");
				}
				else
				{
					embedBuilder.AddField(weapon.WeaponName, $"1d{weapon.DieSize} melee");
				}
			}

			await ReplyAsync(embed: embedBuilder.Build());
		}

		[Command("add weapon")]
		[RequireStewardPermission]
		public async Task AddWeapon(string name, string rangedOrMelee, int dieSize)
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

			if (dieSize < 2 || dieSize > 20)
			{
				await ReplyAsync("The die size has to be within the 2 to 20 range.");
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
				DieSize = dieSize,
				IsRanged = rangedOrMelee == "ranged",
				DamageModifier = CharacterAttribute.STR
			};

			_stewardContext.ValkFinderWeapons.Add(newWeapon);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Weapon added.");
		}
	}
}