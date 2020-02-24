using Discord.Commands;
using Discord.WebSocket;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Steward.Discord
{
	public class CommandHandler
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly IServiceProvider _services;

		private StewardContext _stewardContext { get; set; }

		public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client, StewardContext stewardContext)
		{
			_commands = commands;
			_services = services;
			_client = client;
			_stewardContext = stewardContext;

			_client.MessageReceived += HandleCommandAsync;
		}

		private async Task HandleCommandAsync(SocketMessage messageParam)
		{
			var message = messageParam as SocketUserMessage;
			if (message == null) return;

			int argPos = 0;

			if (!(message.HasCharPrefix('&', ref argPos) ||
				message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
				message.Author.IsBot)
				return;

			//checks if the user has a profile
			NewProfile(message.Author.Id);

			var context = new SocketCommandContext(_client, message);

			var result = await _commands.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: _services);

			if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
			//if (!result.IsSuccess) await context.Channel.SendMessageAsync(InsultList[new Random().Next(InsultList.Count)]);
		}

		private List<string> InsultList = new List<string>()
		{
			"YOU FOOL, YOU ABSOLUTE BUFFOON",
			"I knew a Feudal season would be too complex for you smoothbrains",
			"Fuck you",
			"You're worse than those people with Sonic OCs",
			"Your Father was a Hamster and your Mother Smelt of Elderberries!",
			"You waste my time",
			"I hate so many of the things you choose to be",
			"Thou inbred bastard!",
			"Oh, it's you again",
			"Does Barry Manilow know that you raid his wardrobe?",
			"Ugh",
			"You type like old people fuck",
			"You're what the French call: 'les incompetents'.",
			"Were you always this stupid or did you take lessons?",
			"Are you a special agent sent here to ruin my evening and possibly my entire life?",
			"My disappointment is immeasurable, and my day is ruined",
			"Imagine if the holocaust happened every 4 years like the olympics. I would prefer that than to talk with you for another second",
			"Your birth was, and I do not say this lightly, worse than a 100 9/11's",
			"That's going on my cringe compilation."
		};

		private async void NewProfile(ulong id)
		{
			var exists = _stewardContext.DiscordUsers.SingleOrDefault(u => u.DiscordId == id.ToString());

			if(exists == null)
			{
				var newUser = new DiscordUser()
				{
					DiscordId = id.ToString(),
					CanUseAdminCommands = false
				};

				_stewardContext.DiscordUsers.Add(newUser);
				await _stewardContext.SaveChangesAsync();
			}
		}
	}
}
