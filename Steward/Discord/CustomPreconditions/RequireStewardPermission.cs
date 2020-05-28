using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Steward.Context;

namespace Steward.Discord.CustomPreconditions
{
	public class RequireStewardPermission : PreconditionAttribute
	{

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var db = services.GetService(typeof(StewardContext)) as StewardContext;

			var discordUser = db.DiscordUsers.SingleOrDefault(du => du.DiscordId == context.User.Id.ToString());

			if (discordUser == null || !discordUser.CanUseAdminCommands)
			{
				return Task.FromResult(
					PreconditionResult.FromError("You don't have the required permissions to use this command."));
			}

			return Task.FromResult(PreconditionResult.FromSuccess());
		}
	}
}
