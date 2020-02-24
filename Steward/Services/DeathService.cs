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

        private readonly StewardContext _context;
        
        public DeathService(StewardContext stewardContext, DiscordSocketClient client)
        {
            _client = client;
            _context = stewardContext;
        }

        public async Task Kill(ulong id, bool graveyardMessage, ISocketMessageChannel channel)
        {
	        var activeCharacter =
		        _context.PlayerCharacters
			        .Include(c => c.DefaultMeleeWeapon)
			        .SingleOrDefault(c => c.DiscordUserId == id.ToString() && c.IsAlive());

	        if (activeCharacter == null)
            {
	            await channel.SendMessageAsync("This user does not have an active character.");
	            return;
            }

	        var graveYards = _context.Graveyards.ToList();

	        var year = _context.Year.SingleOrDefault();

            activeCharacter.YearOfDeath = year.CurrentYear.ToString();

            if (graveyardMessage)
            {
	            activeCharacter.YearOfDeath = "???";
	            var message = $"{activeCharacter.CharacterName} {activeCharacter.YearOfBirth} - {activeCharacter.YearOfDeath}";

	            foreach (var graveyardChannel in graveYards.Select(graveyard => _client.GetChannel(ulong.Parse(graveyard.ChannelId)) as SocketTextChannel))
	            {
		            try
		            {
			            await graveyardChannel.SendMessageAsync(message);
                    }
		            catch
		            {
                        //nothing, I just don't want it to crash the command
		            }
	            }
            }

            _context.PlayerCharacters.Update(activeCharacter);
            _context.SaveChanges();
        }
    }
}
