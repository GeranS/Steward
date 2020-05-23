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
	public class RequireHouseOwner : PreconditionAttribute
	{

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var db = services.GetService(typeof(StewardContext)) as StewardContext;

			var activeCharacter =
				db.PlayerCharacters
					.Include(c => c.House)
					.ThenInclude(h => h.HouseOwner)
					.SingleOrDefault(c => c.DiscordUserId == context.User.Id.ToString() && c.YearOfDeath == null);

			if (activeCharacter == null || !activeCharacter.IsAlive())
			{
				return Task.FromResult(
					PreconditionResult.FromError("Could not find an active character."));
			}

			if (activeCharacter.House == null)
			{
				return Task.FromResult(
					PreconditionResult.FromError("Your character is not part of a house."));
			}

			if (activeCharacter.House.HouseOwner == null || activeCharacter.CharacterId != activeCharacter.House.HouseOwner.CharacterId)
			{
				return Task.FromResult(
					PreconditionResult.FromError("You are not the owner of this house."));
			}

			return Task.FromResult(PreconditionResult.FromSuccess());
		}
	}
}
