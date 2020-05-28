using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
	public class YearModule : ModuleBase<SocketCommandContext>
	{
		private readonly StewardContext _stewardContext;
		private readonly DeathService _deathService;

		public YearModule(StewardContext stewardContext, DeathService deathService)
		{
			_stewardContext = stewardContext;
			_deathService = deathService;
		}

		[Command("year")]
		public async Task GetCurrentYear()
		{
			var year = _stewardContext.Year.SingleOrDefault();

			await ReplyAsync($"The current year is {year.CurrentYear}.");
		}

		[Command("advance")]
		[RequireStewardPermission]
		public async Task AdvanceYear(int amount)
		{
			if (amount <= 0)
			{
				await ReplyAsync("YOU FOOL, YOU ABSOLUTE BUFFOON");
				return;
			}

			var currentYear = _stewardContext.Year.First();

			var charactersOver60AndAlive = _stewardContext.PlayerCharacters.Where(c =>
				c.YearOfDeath == null && c.YearOfBirth < currentYear.CurrentYear - 60);

			Console.WriteLine("Amount of characters checked: " + charactersOver60AndAlive.Count());

			foreach (var character in charactersOver60AndAlive)
			{
				//-1, because else it'll check a year twice
				await _deathService.PerformOldAgeCalculation(character, currentYear.CurrentYear, currentYear.CurrentYear + amount - 1);
			}

			currentYear.CurrentYear += amount;

			_stewardContext.PlayerCharacters.UpdateRange(charactersOver60AndAlive);
			_stewardContext.Year.Update(currentYear);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync($"The year is now {currentYear.CurrentYear}.");
		}
    }
}
