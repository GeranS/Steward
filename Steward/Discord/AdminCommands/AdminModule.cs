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

        public AdminModule(StewardContext stewardContext, DeathService deathService)
        {
            _stewardContext = stewardContext;
            _deathService = deathService;
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
    }
}
