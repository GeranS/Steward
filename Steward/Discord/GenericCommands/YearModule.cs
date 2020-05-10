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
		private StewardContext _stewardContext { get; set; }
		private DeathService _deathService { get; set; }

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

			currentYear.CurrentYear = currentYear.CurrentYear + amount;

			_stewardContext.Year.Update(currentYear);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync($"The year is now {currentYear.CurrentYear}.");
		}
    }
}
