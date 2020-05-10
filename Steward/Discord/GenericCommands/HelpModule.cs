using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Steward.Context;

namespace Steward.Discord.GenericCommands
{
	public class HelpModule : ModuleBase<SocketCommandContext>
	{
		private readonly StewardContext _stewardContext;
		private readonly CommandService _commandService;

		public HelpModule(StewardContext context, CommandService service)
		{
			_stewardContext = context;
			_commandService = service;
		}

		[Command("help")]
		public async Task Help(string commandName)
		{
			var commandList = (List<CommandInfo>)_commandService.Commands.GetEnumerator();

			var command = commandList.Find(cl => cl.Aliases.Contains(commandName));
		}

		[Command("help")]
		public async Task Help()
		{
			var commandList = _commandService.Commands;

			var embedBuilder = new EmbedBuilder().WithTitle("Help").WithColor(Color.Purple);

			var listOfCommandsString = "";

			foreach (var commandInfo in commandList)
			{
				listOfCommandsString += $"{commandInfo.Name}\n";
			}

			embedBuilder.AddField("Commands", listOfCommandsString);

			await ReplyAsync(embed: embedBuilder.Build());
		}
	}
}
