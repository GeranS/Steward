using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
				var descriptionField = house.HouseDescription;
				if (descriptionField.Length > 100)
				{
					descriptionField = descriptionField.Substring(0, 100) + "...";
				}
				embedBuilder.AddField(house.HouseName, descriptionField);
			}

			await ReplyAsync(embed: embedBuilder.Build());
		}

		[Command("house")]
		public async Task ShowHouseStats(string houseName = null)
		{
			House theHouseINeed = null;

			if (houseName != null)
			{
				var house = _stewardContext.Houses.SingleOrDefault(h => h.HouseName == houseName);

				if (house == null)
				{
					var houseNameForReply = houseName;
					if (houseNameForReply.Length > 50)
					{
						houseNameForReply = houseNameForReply.Substring(0, 50) + "..";
					}
					await ReplyAsync($"Could not find house {houseNameForReply}.");
					return;
				}

				theHouseINeed = house;
			}
			else
			{
				var activeCharacter =
					_stewardContext.PlayerCharacters
						.Include(c => c.House)
						.ThenInclude(h => h.HouseOwner)
						.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

				if (activeCharacter == null || !activeCharacter.IsAlive())
				{
					await ReplyAsync("Could not find a living character.");
					return;
				}

				if (activeCharacter.House == null)
				{
					await ReplyAsync("Your active character is not part of any house.");
					return;
				}

				theHouseINeed = activeCharacter.House;
			}

			var embedBuilder = new EmbedBuilder();

			var houseOwnerString = theHouseINeed.HouseOwner.CharacterName ?? "None";

			var embedFieldValueString = $"Owner: {houseOwnerString}\n\nDescription:\n" + theHouseINeed.HouseDescription;

			var bonusString =
				$"\n\nSTR: {theHouseINeed.STR}\nEND: {theHouseINeed.END}\nDEX: {theHouseINeed.DEX}\nPER: {theHouseINeed.PER}\nINT: {theHouseINeed.INT}\nArmor Class Bonus: {theHouseINeed.ArmorClassBonus}"
				+ $"\nAbility Point Bonus: {theHouseINeed.AbilityPointBonus}\nHealth Pool Bonus: {theHouseINeed.HealthPoolBonus}";

			embedBuilder.AddField("House of " + theHouseINeed.HouseName, embedFieldValueString);
			embedBuilder.AddField("Bonuses", bonusString);

			embedBuilder.WithColor(Color.Purple);

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
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

			// Doesn't need a null check, already done in RequireHouseOwner
			var house = activeCharacter.House;

			if (description.Length > 1800)
			{
				await ReplyAsync("Description has to be 1800 characters or less.");
				return;
			}

			house.HouseDescription = description;

			_stewardContext.Update(house);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("House description changed.");
		}

		[Command("own")]
		[RequireStewardPermission]
		public async Task SetHouseOwner([Remainder] SocketGuildUser mention)
		{
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.Include(c => c.House)
					.SingleOrDefault(c => c.DiscordUserId == mention.Id.ToString() && c.YearOfDeath == null);

			if (activeCharacter == null || !activeCharacter.IsAlive())
			{
				await ReplyAsync($"The user doesn't have a living character.");
				return;
			}

			activeCharacter.House.HouseOwner = activeCharacter;

			_stewardContext.Update(activeCharacter.House);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync(
				$"Owner of house {activeCharacter.House.HouseName} has been set to {activeCharacter.CharacterName}.");
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
