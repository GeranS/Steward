using Discord;
using Discord.Commands;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Steward.Discord.CustomPreconditions;

namespace Steward.Discord.GenericCommands
{
	public class StaffActionPublicModule : ModuleBase<SocketCommandContext>
	{
		private readonly StewardContext _stewardContext;

		public StaffActionPublicModule(StewardContext context)
		{
			_stewardContext = context;
		}

		[Command("respond")]
		[RequireStewardPermission]
		public async Task RespondToTask(long id, string response)
		{

		}

		[Command("assign")]
		[RequireStewardPermission]
		public async Task AssignAction(long id, [Remainder] SocketGuildUser mention)
		{
			var staffAction = _stewardContext.StaffActions.SingleOrDefault(sa => sa.StaffActionId == id);

			var mentionedUser = _stewardContext.DiscordUsers.SingleOrDefault(du => du.DiscordId == mention.Id.ToString());

			if (mentionedUser == null)
			{
				await ReplyAsync("Could not find mentioned user.");
				return;
			}

			if (!mentionedUser.CanUseAdminCommands)
			{
				await ReplyAsync("This user is not allowed to use staff commands.");
				return;
			}

			staffAction.AssignedTo = mentionedUser;

			_stewardContext.StaffActions.Update(staffAction);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Task assigned.");
		}

		[Command("submit")]
		public async Task SubmitAction(string title, string description)
		{
			var discordUser = _stewardContext.DiscordUsers.SingleOrDefault(du => du.DiscordId == Context.User.Id.ToString());

			if (title.Length > 200)
			{
				await ReplyAsync("Title must be shorter than 200 characters.");
				return;
			}

			if (description.Length > 1800)
			{
				await ReplyAsync("Description must be shorter than 1800 characters.");
				return;
			}

			StaffAction action = new StaffAction()
			{
				ActionTitle = title,
				ActionDescription = description,
				Status = StaffActionStatus.TODO,
				SubmitterId = discordUser.DiscordId,
				Submitter = discordUser
			};

			_stewardContext.StaffActions.Add(action);
			_stewardContext.SaveChanges();

			await ReplyAsync("Staff action submitted.");
		}

		[Command("actions")]
		public async Task AllActions()
		{
			var activeActions = _stewardContext.StaffActions.Where(sa => sa.Status != StaffActionStatus.DONE);

			var embedBuilder = new EmbedBuilder
			{
				Color = Color.Purple
			};

			if (!activeActions.Any())
			{
				await ReplyAsync("No actions found.");
				return;
			}

			foreach (var sa in activeActions)
			{
				var embedFieldBuilder = new EmbedFieldBuilder
				{
					Value = sa.ActionTitle,
					Name = sa.Status.ToString(),
					IsInline = false
				};

				embedBuilder.AddField(embedFieldBuilder);
			}

			await ReplyAsync("", false, embedBuilder.Build(), null);
		}

		[Command("my actions")]
		public async Task MyActions()
		{
			var activeActions = _stewardContext.StaffActions.Where(sa => sa.SubmitterId == Context.User.Id.ToString() && sa.Status != StaffActionStatus.DONE);

			var embedBuilder = new EmbedBuilder
			{
				Color = Color.Purple
			};

			if (!activeActions.Any())
			{
				await ReplyAsync("No actions found.");
				return;
			}

			foreach (var sa in activeActions)
			{
				var embedFieldBuilder = new EmbedFieldBuilder
				{
					Value = sa.ActionTitle,
					Name = sa.Status.ToString(),
					IsInline = false
				};

				embedBuilder.AddField(embedFieldBuilder);
			}

			await ReplyAsync("", false, embedBuilder.Build(), null);
		}
	}
}
