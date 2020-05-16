using Discord.Commands;
using Discord.WebSocket;
using Steward.Context;
using Steward.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Microsoft.EntityFrameworkCore;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;

namespace Steward.Discord.AdminCommands
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
	    private StewardContext _stewardContext { get; set; }
        private DeathService _deathService { get; set; }

		private readonly DiscordSocketClient _client;

		public AdminModule(StewardContext stewardContext, DeathService deathService, DiscordSocketClient client)
        {
            _stewardContext = stewardContext;
            _deathService = deathService;
			_client = client;
        }

        [Command("raise dead")]
        [RequireStewardPermission]
        public async Task RemoveGraveyard()
        {
	        var existingYard = _stewardContext.Graveyards.FirstOrDefault(gy => gy.ChannelId == Context.Channel.Id.ToString());

	        if (existingYard == null)
	        {
		        await ReplyAsync("There's no graveyard in this channel.");
		        return;
	        }

	        _stewardContext.Graveyards.Remove(existingYard);
	        await _stewardContext.SaveChangesAsync();

	        await ReplyAsync("Graveyard removed.");
        }

        [Command("add graveyard")]
        [RequireStewardPermission]
        public async Task AddGraveyard()
        {
	        var existingYard = _stewardContext.Graveyards.FirstOrDefault(gy => gy.ChannelId == Context.Channel.Id.ToString());

	        if (existingYard != null)
	        {
		        await ReplyAsync("This channel is already a graveyard. If you want to remove this graveyard, use 'raise dead'");
		        return;
	        }

            var newGraveyard = new Graveyard()
            {
                ChannelId = Context.Channel.Id.ToString(),
                ServerId = Context.Guild.Id.ToString()
            };

            await _stewardContext.Graveyards.AddAsync(newGraveyard);
            await _stewardContext.SaveChangesAsync();

            await ReplyAsync("Graveyard added.");
        }

        [Command("reset")]
        [RequireStewardPermission]
        public async Task Reset([Remainder] SocketGuildUser mention)
        {
	        await _deathService.Kill(mention.Id, false, Context.Channel);
	        await ReplyAsync($"Press F for {mention}.");
        }

        [Command("kill")]
        [RequireStewardPermission]
        public async Task Kill([Remainder]SocketGuildUser mention)
        {
	        var author = _stewardContext.DiscordUsers.SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

	        if (!author.CanUseAdminCommands)
	        {
		        await ReplyAsync("You don't have access to this command");
                return;
	        }

            await _deathService.Kill(mention.Id, true, Context.Channel);
            await ReplyAsync("Press F.");
        }

        [Command("set age")]
        [RequireStewardPermission]
        public async Task SetAge(int newAge, [Remainder] SocketGuildUser mention)
        {
	        if (newAge < 18)
	        {
		        await ReplyAsync("Can't set age below 18");
		        return;
	        }

	        if (newAge > 100)
	        {
		        await ReplyAsync("Can't set age over 100");
		        return;
	        }

	        var activeCharacter =
		        _stewardContext.PlayerCharacters
			        .SingleOrDefault(c => c.DiscordUserId == mention.Id.ToString() && c.YearOfDeath == null);

	        if (activeCharacter == null)
	        {
		        await ReplyAsync("Could not find a living character.");
		        return;
	        }

	        var year = _stewardContext.Year.First();

	        activeCharacter.InitialAge = newAge;

	        var newYearOfBirth = year.CurrentYear - newAge;

            activeCharacter.YearOfBirth = newYearOfBirth;

            _stewardContext.PlayerCharacters.Update(activeCharacter);
            await _stewardContext.SaveChangesAsync();

            await ReplyAsync("Age changed.");
		}

        [Command("op")]
        [RequireStewardPermission]
        public async Task OpUser([Remainder] SocketGuildUser mention)
        {
	        var discordUser = _stewardContext.DiscordUsers.SingleOrDefault(du => du.DiscordId == mention.Id.ToString());

	        if (discordUser == null)
	        {
		        await ReplyAsync("User does not have a profile.");
                return;
	        }

	        if (discordUser.CanUseAdminCommands)
	        {
		        await ReplyAsync("User is already an admin.");
                return;
	        }

	        discordUser.CanUseAdminCommands = true;

	        _stewardContext.DiscordUsers.Update(discordUser);
	        await _stewardContext.SaveChangesAsync();

	        await ReplyAsync("User has been made an admin.");
        }

        [Command("deop")]
        [RequireStewardPermission]
        public async Task DeOpUser([Remainder] SocketGuildUser mention)
        {
	        var discordUser = _stewardContext.DiscordUsers.SingleOrDefault(du => du.DiscordId == mention.Id.ToString());

	        if (!discordUser.CanUseAdminCommands)
	        {
		        await ReplyAsync("User is not an admin.");
                return;
	        }

            discordUser.CanUseAdminCommands = false;

	        _stewardContext.DiscordUsers.Update(discordUser);
	        await _stewardContext.SaveChangesAsync();

	        await ReplyAsync("Admin privileges have been removed.");
        }

		[Command("list op")]
		[RequireStewardPermission]
		public async Task ListOp()
		{
			var DiscordUsers = await _stewardContext.DiscordUsers.ToListAsync();
			var Message = "";

			foreach (var DiscordUser in _stewardContext.DiscordUsers)
			{
				if (DiscordUser.CanUseAdminCommands == true)
				{
					
					var UserName = _client.GetUser(ulong.Parse(DiscordUser.DiscordId)).ToString();
					Message += UserName + " ; ";
				}
			}

			await ReplyAsync(Message);
		}
    }
}
