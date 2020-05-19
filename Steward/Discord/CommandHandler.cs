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

		private readonly StewardContext _stewardContext;

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

			//checks if the user has a profile
			NewProfile(message.Author.Id);

			if (!(message.HasCharPrefix('&', ref argPos) ||
				message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
				message.Author.IsBot)
				return;

			var context = new SocketCommandContext(_client, message);

			var result = await _commands.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: _services);

			if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
			//if (!result.IsSuccess) await context.Channel.SendMessageAsync(InsultList[new Random().Next(InsultList.Count)]);
		}

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
