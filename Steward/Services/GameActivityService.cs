using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Steward.Services
{
	public class GameActivityService
	{
		private readonly IOptions<StewardConfig> _config;
		private readonly DiscordSocketClient _client;

		public GameActivityService(IOptions<StewardConfig> config, DiscordSocketClient client)
		{
			_config = config;
			_client = client;

			_client.SetGameAsync(_config.Value.Version);
		}
	}
}
