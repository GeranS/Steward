
using System;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Hosting.Self;
using Steward.Context;
using Steward.Discord;
using Steward.Discord.AdminCommands;
using Steward.Discord.GenericCommands;
using Steward.Services;

namespace Steward
{
	class Program
	{

		private readonly CommandService _commands = new CommandService();
		private readonly DiscordSocketClient _client = new DiscordSocketClient();
		private IServiceProvider _services;

		static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

		public async Task Start()
		{
			_client.Log += Log;

			_services = BuildServiceProvider();

			await _client.LoginAsync(TokenType.Bot, Properties.Resources.token);
			await _client.StartAsync();

			new CommandHandler(_services, _commands, _client, _services.GetService<StewardContext>());

			await _commands.AddModulesAsync(
			assembly: Assembly.GetEntryAssembly(), 
			services: _services);

			foreach(var x in _commands.Commands)
			{
				Console.WriteLine("Command: " + x.Name.ToString());
			}

			await _client.SetGameAsync("v0_2");

			//using (var host = new NancyHost(new Uri("http://localhost:1234")))
			//{
			//	host.Start();
			//	Console.WriteLine("Running on http://localhost:1234");
			//	Console.ReadLine();
			//}

			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		public IServiceProvider BuildServiceProvider() => new ServiceCollection()
			.AddSingleton(_client)
			.AddSingleton(_commands)
			.AddSingleton<RollService>()
			.AddSingleton<ActivityService>()
			.AddSingleton<DeathService>()
			.AddSingleton<CharacterService>()
			.AddSingleton<HouseRoleManager>()
			.AddDbContext<StewardContext>(ServiceLifetime.Transient)
			.BuildServiceProvider();
	}
}
