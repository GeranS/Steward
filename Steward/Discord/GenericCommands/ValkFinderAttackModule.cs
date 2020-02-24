﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Steward.Context;
using Steward.Context.Models;
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

		[Command("melee default")]
		public async Task SetDefaultMeleeWeapon(string weaponName)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.DefaultMeleeWeapon)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.IsAlive());

			if (activeCharacter == null)
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
					.Include(c => c.CharacterTraits)
					.ThenInclude(ct => ct.Trait)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.IsAlive());

			if (activeCharacter == null)
			{
				await ReplyAsync($"You don't have a living character.");
				return;
			}

			ValkFinderWeapon valkFinderWeapon = null;

			valkFinderWeapon = activeCharacter.DefaultMeleeWeapon ?? _stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == weaponName);

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

		[Command("ranged default")]
		public async Task SetDefaultRangedWeapon(string weaponName)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.DefaultRangedWeapon)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.IsAlive());

			if (activeCharacter == null)
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

			activeCharacter.DefaultMeleeWeapon = valkFinderWeapon;

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
					.Include(c => c.CharacterTraits)
					.ThenInclude(ct => ct.Trait)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.IsAlive());

			if (activeCharacter == null)
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
	}
}