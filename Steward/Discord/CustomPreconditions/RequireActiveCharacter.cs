using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Steward.Context;

namespace Steward.Discord.CustomPreconditions
{
	public class RequireActiveCharacter : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var db = services.GetService(typeof(StewardContext)) as StewardContext;

			var activeCharacter =
				db.PlayerCharacters
					.SingleOrDefault(c => c.DiscordUserId == context.User.Id.ToString() && c.YearOfDeath == null);

			return Task.FromResult(activeCharacter == null ? PreconditionResult.FromError("Could not find an active character.") : PreconditionResult.FromSuccess());
		}
	}
}
