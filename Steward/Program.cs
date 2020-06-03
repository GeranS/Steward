
using System;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Hosting.Self;
using Steward.Context;
using Steward.Discord;
using Steward.Discord.AdminCommands;
using Steward.Discord.GenericCommands;
using Steward.Services;

namespace Steward
{
	public class Program
	{

		private readonly CommandService _commands = new CommandService();
		private readonly DiscordSocketClient _client = new DiscordSocketClient();
		private IServiceProvider _services;

		public static IConfigurationRoot Configuration { get; set; }

		static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

		public async Task Start()
		{
			var configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

			Configuration = configBuilder.Build();

			_client.Log += Log;

			_services = BuildServiceProvider();

			//var stewardConfig = Configuration.GetSection("StewardConfig").Get<StewardConfig>();
			var stewardConfig = new StewardConfig();
			Configuration.GetSection("StewardConfig").Bind(stewardConfig);

			Console.WriteLine("Token:" + stewardConfig.Token);

			await _client.LoginAsync(TokenType.Bot, stewardConfig.Token);
			await _client.StartAsync();

			new CommandHandler(_services, _commands, _client, _services.GetService<StewardContext>());

			await _commands.AddModulesAsync(
			assembly: Assembly.GetEntryAssembly(),
			services: _services);

			foreach (var x in _commands.Commands)
			{
				Console.WriteLine("Command: " + x.Name.ToString());
			}

			await _client.SetGameAsync(stewardConfig.Version);

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

		public IServiceProvider BuildServiceProvider()
		{
			var services = new ServiceCollection();

			services.AddOptions();

			services.Configure<StewardConfig>(Configuration.GetSection("StewardConfig"));

			services.AddSingleton(_client)
			.AddSingleton(_commands)
			.AddSingleton<RollService>()
			.AddSingleton<ActivityService>()
			.AddSingleton<DeathService>()
			.AddSingleton<CharacterService>()
			.AddSingleton<HouseRoleManager>()
			.AddSingleton<GameActivityService>()
			.AddSingleton<StaffActionService>()
			.AddSingleton<InventoryService>()
			.AddDbContext<StewardContext>(ServiceLifetime.Transient)
			.BuildServiceProvider();

			return services.BuildServiceProvider();
		}
	}
}
