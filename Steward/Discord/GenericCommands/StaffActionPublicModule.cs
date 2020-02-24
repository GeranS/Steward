using Discord;
using Discord.Commands;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steward.Discord.GenericCommands
{
	public class StaffActionPublicModule : ModuleBase<SocketCommandContext>
	{
		private readonly StewardContext _context;

		public StaffActionPublicModule(StewardContext context)
		{
			_context = context;
		}

		[Command("do")]
		public async Task Do(string description)
		{
			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordId == Context.User.Id.ToString());

			StaffAction action = new StaffAction()
			{
				ActionDescription = description,
				Status = StaffActionStatus.TODO,
				SubmitterId = discordUser.DiscordId,
				Submitter = discordUser
			};

			_context.StaffActions.Add(action);
			_context.SaveChanges();

			await ReplyAsync("Staff action submitted.");
		}

		[Command("actions")]
		public async Task MyActions()
		{
			var activeActions = _context.StaffActions.Where(sa => sa.SubmitterId == Context.User.Id.ToString() && sa.Status != StaffActionStatus.DONE);

			var embedBuilder = new EmbedBuilder
			{
				Color = Color.Purple
			};

			foreach (var sa in activeActions)
			{
				var embedFieldBuilder = new EmbedFieldBuilder
				{
					Value = sa.ActionDescription,
					Name = sa.Status.ToString(),
					IsInline = false
				};

				embedBuilder.AddField(embedFieldBuilder);
			}

			await ReplyAsync("", false, embedBuilder.Build(), null);
		}
	}
}
