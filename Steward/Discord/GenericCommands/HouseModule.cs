using System;
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

namespace Steward.Discord.GenericCommands
{
	public class HouseModule : ModuleBase<SocketCommandContext>
	{
		private readonly StewardContext _stewardContext;

		public HouseModule(StewardContext stewardContext)
		{
			_stewardContext = stewardContext;
		}

		[Command("houses")]
		public async Task ShowHouses()
		{
			var houses = _stewardContext.Houses.ToList();

			var embedBuilder = new EmbedBuilder().WithColor(Color.Purple);

			foreach (var house in houses)
			{
				embedBuilder.AddField(house.HouseName, house.HouseDescription);
			}

			await ReplyAsync(embed: embedBuilder.Build());
		}

		[Command("house stats")]
		[RequireStewardPermission]
		public async Task ChangeHouseTraits(
			string houseName,
			int str,
			int dex,
			int end,
			int per,
			int intel,
			int armorClass,
			int healthPool,
			int abilityPoint)
		{
			var house = _stewardContext.Houses.FirstOrDefault(h => h.HouseName == houseName);

			if (house == null)
			{
				await ReplyAsync("Could not find house.");
				return;
			}

			house.STR = str;
			house.DEX = dex;
			house.END = end;
			house.PER = per;
			house.INT = intel;

			house.ArmorClassBonus = armorClass;
			house.HealthPoolBonus = healthPool;
			house.AbilityPointBonus = abilityPoint;

			_stewardContext.Houses.Update(house);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("House stats updated.");
		}

		[Command("house desc")]
		[RequireHouseOwner]
		public async Task ChangeHouseDescription(string description)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.House)
					.ThenInclude(h => h.HouseOwner)
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.IsAlive());

			// Doesn't need a null check, already done in RequireHouseOwner
			var house = activeCharacter.House;

			if (description.Length > 1800)
			{
				await ReplyAsync("Description has to be 1800 characters or less.");
				return;
			}

			house.HouseDescription = description;

			_stewardContext.Update(house);

			await ReplyAsync("House description changed.");
		}

		[Command("add house")]
		[RequireStewardPermission]
		public async Task AddHouse(string name)
		{
			var existingHouse =
				_stewardContext.Houses.SingleOrDefault(h => h.HouseName == name);

			if (existingHouse != null)
			{
				await ReplyAsync("This house already exists.");
				return;
			}

			var newHouse = new House()
			{
				HouseName = name
			};

			await _stewardContext.Houses.AddAsync(newHouse);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync($"House with the name {name} has been created.");
		}
	}
}
