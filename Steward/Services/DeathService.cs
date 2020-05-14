using Discord.WebSocket;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Steward.Services
{
    public class DeathService
    {
        private readonly DiscordSocketClient _client;

        private readonly StewardContext _stewardContext;
        
        public DeathService(StewardContext stewardContext, DiscordSocketClient client)
        {
            _client = client;
            _stewardContext = stewardContext;
        }

        public async Task Kill(ulong id, bool graveyardMessage, ISocketMessageChannel channel)
        {
	        var activeCharacter =
		        _stewardContext.PlayerCharacters
			        .SingleOrDefault(c => c.DiscordUserId == id.ToString() && c.YearOfDeath == null);

            if (activeCharacter == null || !activeCharacter.IsAlive())
            {
	            await channel.SendMessageAsync("This user does not have an active character.");
	            return;
            }

	        var graveYards = _stewardContext.Graveyards.ToList();

	        var year = _stewardContext.Year.SingleOrDefault();

            activeCharacter.YearOfDeath = year.CurrentYear.ToString();

            if (graveyardMessage)
            {
	            var message = $"{activeCharacter.CharacterName} {activeCharacter.YearOfBirth} - {activeCharacter.YearOfDeath}";

	            foreach (var graveyardChannel in graveYards.Select(graveyard => _client.GetChannel(ulong.Parse(graveyard.ChannelId)) as SocketTextChannel))
	            {
		            try
		            {
			            await graveyardChannel.SendMessageAsync(message);
                    }
		            catch(NullReferenceException e)
		            {
                        Console.WriteLine(e.StackTrace);
                        //nothing, I just don't want it to crash the command
		            }
	            }
            }

            _stewardContext.PlayerCharacters.Update(activeCharacter);
            _stewardContext.SaveChanges();
        }
    }
}
