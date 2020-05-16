using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Steward.Context;
using Steward.Context.Models;

namespace Steward.Services
{
	public class HouseRoleManager
	{
		private readonly DiscordSocketClient _client;

		private readonly StewardContext _stewardContext;

		public HouseRoleManager(StewardContext stewardContext, DiscordSocketClient client)
		{
			_client = client;
			_stewardContext = stewardContext;
		}

		//returns null on success
		public async Task<string> UpdatePlayerHouseRole(PlayerCharacter character, IEnumerable<House> houses)
		{
			var house = character.House;

			var guild = _client
				.GetGuild(ulong.Parse(Properties.Resources.guildId));
			
			SocketRole houseRole;
			try
			{
				houseRole = guild.GetRole(ulong.Parse(house.HouseRoleId));
			}
			catch
			{
				return $"Could not find house role for {house.HouseName}.";
			}
			
			var allHouseRoles = new List<SocketRole>();

			foreach (var h in houses)
			{
				try
				{
					allHouseRoles.Add(guild.GetRole(ulong.Parse(h.HouseRoleId)));
				}
				catch
				{
					return $"Could not find house role for {h.HouseName}.";
				}
			}

			var allHouseRolesExceptNeededOne = new List<SocketRole>(allHouseRoles);
			allHouseRolesExceptNeededOne.Remove(houseRole);

			var rolesToBeRemoved = new List<SocketRole>();

			var guildUser = guild.GetUser(ulong.Parse(character.DiscordUserId));

			foreach (var unwantedHouseRole in allHouseRolesExceptNeededOne)
			{
				if (guildUser.Roles.Contains(unwantedHouseRole))
				{
					rolesToBeRemoved.Add(unwantedHouseRole);
				}
			}

			try
			{
				await guildUser.RemoveRolesAsync(rolesToBeRemoved);

				if (!guildUser.Roles.Contains(houseRole))
				{
					await guildUser.AddRoleAsync(houseRole);
				}
			}
			catch
			{
				return "Could not assign/remove roles.";
			}

			return null;
		}
	}
}
