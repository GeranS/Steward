using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
    public class MarriageModule: ModuleBase<SocketCommandContext>
    {
		private readonly StewardContext _stewardContext;
		private readonly DiscordSocketClient _client;

		public MarriageModule(StewardContext context, DiscordSocketClient client)
		{
			_stewardContext = context;
			_client = client;


		}

		public async Task MarryPlayer([Remainder]SocketGuildUser mention)
		{
            var commandUser = _stewardContext.DiscordUsers
                    .Include(du => du.Characters)
                    .SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

            var activeCharacter = commandUser.Characters.Find(c => c.IsAlive());

            if (activeCharacter == null)
            {
                await ReplyAsync("Could not find a living character.");
                return;
            }
            var discordUser = _stewardContext.DiscordUsers
                    .Include(du => du.Characters)
                    .SingleOrDefault(u => u.DiscordId == mention.Id.ToString());

            var recievingCharacter = discordUser.Characters.Find(c => c.IsAlive());

            if (recievingCharacter == null)
            {
                await ReplyAsync($"Could not find a living character for {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}.");
                return;
            }

            var activeSpouse = _stewardContext.PlayerCharacters.FirstOrDefault(c => c.CharacterId == activeCharacter.SpouseId);  

            if (activeSpouse != null && activeSpouse.IsAlive())
            {
                await ReplyAsync("You are already married!");
                return;
            }

            var recieverSpouse = _stewardContext.PlayerCharacters.FirstOrDefault(c => c.CharacterId == recievingCharacter.SpouseId);

            if (recieverSpouse != null && recieverSpouse.IsAlive())
            {
                await ReplyAsync($"{_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()} is already married. Kill his spouse first to marry her/him");
                return;
            }

            activeCharacter.SpouseId = recievingCharacter.CharacterId;
            recievingCharacter.SpouseId = activeCharacter.CharacterId;

            await ReplyAsync($"The Husband and Bride may now kiss!");
        }
	}
}
