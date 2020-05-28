using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Steward.Context;
using Steward.Context.Models;

namespace Steward.Services
{
	public class HouseRoleManager
	{
		private readonly DiscordSocketClient _client;
		private readonly IOptions<StewardConfig> _config;
		private readonly StewardContext _stewardContext;

		public HouseRoleManager(StewardContext stewardContext, IOptions<StewardConfig> config, DiscordSocketClient client)
		{
			_client = client;
			_config = config;
			_stewardContext = stewardContext;
		}

		//returns null on success
		public async Task<string> UpdatePlayerHouseRole(PlayerCharacter character, IEnumerable<House> houses)
		{
			var house = character.House;

			if (house == null)
			{
				return null;
			}

			var guild = _client
				.GetGuild(ulong.Parse(_config.Value.GuildId));
			
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
					Debug.WriteLine($"Could not find a role for {h.HouseName}");
				}
			}

			var guildUser = guild.GetUser(ulong.Parse(character.DiscordUserId));

			if (!character.IsAlive())
			{
				var houseRolesToBeRemoved = new List<SocketRole>();

				foreach (var hr in allHouseRoles)
				{
					if (guildUser.Roles.Contains(hr))
					{
						houseRolesToBeRemoved.Add(hr);
					}
				}

				guildUser.RemoveRolesAsync(houseRolesToBeRemoved);
				return null;
			}

			var allHouseRolesExceptNeededOne = new List<SocketRole>(allHouseRoles);
			allHouseRolesExceptNeededOne.Remove(houseRole);

			var rolesToBeRemoved = allHouseRolesExceptNeededOne.Where(unwantedHouseRole => guildUser.Roles.Contains(unwantedHouseRole)).ToList();

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
