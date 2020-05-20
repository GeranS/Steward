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
		public async Task Help()
		{
			await ReplyAsync("https://docs.google.com/document/d/1w7ICMGPEAHbktlFksIb9uhwwwAkb9obXHXdoAsT_nl8/edit?usp=sharing");
		}
	}
}
