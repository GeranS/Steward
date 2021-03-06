﻿using Discord.WebSocket;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace Steward.Services
{
    public class DeathService
    {
        private readonly DiscordSocketClient _client;

        private readonly StewardContext _stewardContext;
        private readonly HouseRoleManager _houseRoleManager;
        
        public DeathService(StewardContext stewardContext, DiscordSocketClient client, HouseRoleManager houseRoleManager)
        {
            _client = client;
            _stewardContext = stewardContext;
            _houseRoleManager = houseRoleManager;

            StartOldAgeCheckerTimer();
        }

        private void StartOldAgeCheckerTimer()
        {
            var timer = new Timer(30000);
            timer.Elapsed += OldAgeDeathCheck;
            timer.Enabled = true;
        }

        private async void OldAgeDeathCheck(Object source, ElapsedEventArgs e)
        {
	        var charactersToKill = _stewardContext.CharacterDeathTimers
		        .Include(cdt => cdt.PlayerCharacter)
		        .Where(cdt => cdt.DeathTime < DateTime.UtcNow).ToList();

	        foreach (var characterTimer in charactersToKill)
	        {
		        characterTimer.PlayerCharacter.YearOfDeath = characterTimer.YearOfDeath.ToString();

		        _stewardContext.PlayerCharacters.Update(characterTimer.PlayerCharacter);
		        _stewardContext.CharacterDeathTimers.Remove(characterTimer);

		        try
		        {
			        var discordUser = _client.GetUser(ulong.Parse(characterTimer.PlayerCharacter.DiscordUserId));

			        await discordUser.SendMessageAsync($"Your 24 hours are up. {characterTimer.PlayerCharacter.CharacterName} is dead.");
		        }
		        catch (Exception ex)
		        {
			        Console.WriteLine("Exception occurred whilst trying to inform user that their character has died of old age. Exception: " + ex.Message);
				}

		        await SendGraveyardMessage(characterTimer.PlayerCharacter);
	        }
	        await _stewardContext.SaveChangesAsync();
        }

        public async Task PerformOldAgeCalculation(PlayerCharacter character, int startYear, int endYear)
        {
	        var alreadyHasTimer =
		        _stewardContext.CharacterDeathTimers.SingleOrDefault(cdt =>
			        cdt.PlayerCharacterId == character.CharacterId);

	        if (alreadyHasTimer != null)
	        {
		        return;
	        }

            for (var year = startYear; year < endYear +1 ; year++)
	        {
		        Console.WriteLine("YEAR CHECK! " + year);
                var age = character.GetAge(year);

		        var yearsOver60 = age - 60;

		        var chanceOfDeath = yearsOver60 * 2;

                //Cap of 10%
		        if (chanceOfDeath > 10) chanceOfDeath = 10;

                var randomValue = new Random().Next(100);

                if (randomValue < chanceOfDeath)
                {
	                var timer = new CharacterDeathTimer()
	                {
                        PlayerCharacterId = character.CharacterId,
                        YearOfDeath = year,
                        DeathTime = DateTime.UtcNow.AddDays(1)
	                };

	                try
	                {
		                var discordUser = _client.GetUser(ulong.Parse(character.DiscordUserId));

		                await discordUser.SendMessageAsync(
			                $"You feel your life force waning. You are certain you only have 24 hours left, it is best to deal with everything undone now before it's too late.");
	                }
	                catch (Exception e)
	                {
						Console.WriteLine("Exception occurred whilst trying to inform user that their character has 24 hours left. Exception: " + e.Message);
	                }

	                _stewardContext.CharacterDeathTimers.Add(timer);
	                await _stewardContext.SaveChangesAsync();
	                break;
                }
	        }
        }

        private async Task SendGraveyardMessage(PlayerCharacter activeCharacter)
        {
	        var graveYards = _stewardContext.Graveyards.ToList();

            var message = $"{activeCharacter.CharacterName} {activeCharacter.YearOfBirth} - {activeCharacter.YearOfDeath}";

	        foreach (var graveyardChannel in graveYards.Select(graveyard => _client.GetChannel(ulong.Parse(graveyard.ChannelId)) as SocketTextChannel))
	        {
		        try
		        {
			        await graveyardChannel.SendMessageAsync(message);
		        }
		        catch (NullReferenceException e)
		        {
			        Console.WriteLine(e.StackTrace);
			        //nothing, I just don't want it to crash the command
		        }
	        }
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

            var year = _stewardContext.Year.SingleOrDefault();

            activeCharacter.YearOfDeath = year.CurrentYear.ToString();

            if (graveyardMessage)
            {
	            await SendGraveyardMessage(activeCharacter);
            }

            _stewardContext.PlayerCharacters.Update(activeCharacter);
            await _stewardContext.SaveChangesAsync();

            await _houseRoleManager.UpdatePlayerHouseRole(activeCharacter, _stewardContext.Houses.ToList());
        }
    }
}
