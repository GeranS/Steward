using Discord;
using Discord.Commands;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
	public class StaffActionPublicModule : ModuleBase<SocketCommandContext>
	{
		private readonly StewardContext _stewardContext;
		private readonly StaffActionService _staffActionService;
		private readonly DiscordSocketClient _client;

		public StaffActionPublicModule(StewardContext context, StaffActionService s, DiscordSocketClient client)
		{
			_stewardContext = context;
			_staffActionService = s;
			_client = client;
		}

		[Command("create staffaction channel")]
		[RequireStewardPermission]
		public async Task createStaffActionChannel()
		{
			var existingStaffActionChannel = _stewardContext.StaffActionChannels.ToList();

			if (existingStaffActionChannel.Count != 0)
			{
				await ReplyAsync("There's already an existing StaffAction channel.");
				return;
			}

			var newStaffActionChannel = new StaffActionChannel()
			{
				ChannelId = Context.Channel.Id.ToString(),
				ServerId = Context.Guild.Id.ToString()
			};

			await _stewardContext.StaffActionChannels.AddAsync(newStaffActionChannel);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Staff Action Channel added!");
		}

		[Command("respond")]
		[RequireStewardPermission]
		public async Task RespondToTask(long id, string stringStatus, string response)
		{
			var staffAction = _stewardContext.StaffActions.SingleOrDefault(sa => sa.StaffActionId == id);
			
			if (staffAction == null)
			{
				await ReplyAsync($"No Staff Action with ID: {id.ToString()}");
			}
			if (response.Length > 1800)
			{
				await ReplyAsync("Description must be shorter than 1800 characters.");
				return;
			}
			if (!Enum.TryParse(stringStatus, true, out StaffActionStatus status))
			{
				await ReplyAsync("Not a valid attribute.");
				return;
			}


			staffAction.Status = status;
			staffAction.ActionResponse = response;

			_stewardContext.StaffActions.Update(staffAction);
			await _stewardContext.SaveChangesAsync();

			var StaffChannels = _stewardContext.StaffActionChannels.ToList();
			var embedBuilder = _staffActionService.BuildStaffActionMessage(staffAction);
			foreach (var staffChannel in StaffChannels.Select(staffchannel => _client.GetChannel(ulong.Parse(staffchannel.ChannelId)) as SocketTextChannel))
			{
				try
				{
					var message = await staffChannel.GetMessageAsync(ulong.Parse(staffAction.MessageId)) as RestUserMessage;
					await message.ModifyAsync(x => x.Embed = embedBuilder.Build());
				}
				catch (NullReferenceException e)
				{
					Console.WriteLine(e.StackTrace);
					//nothing, I just don't want it to crash the command
				}
			}

			await ReplyAsync("Response received.");
		}

		[Command("assign")]
		[RequireStewardPermission]
		public async Task AssignAction(long id, [Remainder] SocketGuildUser mention)
		{
			var staffAction = _stewardContext.StaffActions.SingleOrDefault(sa => sa.StaffActionId == id);

			var mentionedUser = _stewardContext.DiscordUsers.SingleOrDefault(du => du.DiscordId == mention.Id.ToString());

			if (staffAction == null)
			{
				await ReplyAsync($"No Staff Action with ID: {id.ToString()}");
			}

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

			var StaffChannels = _stewardContext.StaffActionChannels.ToList();
			var embedBuilder = _staffActionService.BuildStaffActionMessage(staffAction);

			foreach (var staffChannel in StaffChannels.Select(staffchannel => _client.GetChannel(ulong.Parse(staffchannel.ChannelId)) as SocketTextChannel))
			{
				try
				{
					var message = await staffChannel.GetMessageAsync(ulong.Parse(staffAction.MessageId)) as RestUserMessage;
					await message.ModifyAsync(x => x.Embed = embedBuilder.Build());
				}
				catch (NullReferenceException e)
				{
					Console.WriteLine(e.StackTrace);
					//nothing, I just don't want it to crash the command
				}
			}
			await ReplyAsync($"Asigned Staff Action to {mention}");
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

			if (description.Length > 1000)
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
			await _stewardContext.SaveChangesAsync();

			var StaffChannels = _stewardContext.StaffActionChannels.ToList();

			var embedBuilder = _staffActionService.BuildStaffActionMessage(action);

			foreach (var staffChannel in StaffChannels.Select(staffchannel => _client.GetChannel(ulong.Parse(staffchannel.ChannelId)) as SocketTextChannel))
			{
				try
				{
					var message = await staffChannel.SendMessageAsync("", false, embedBuilder.Build());
					action.MessageId = message.Id.ToString();
				}
				catch (NullReferenceException e)
				{
					Console.WriteLine(e.StackTrace);
					//nothing, I just don't want it to crash the command
				}
			}

			_stewardContext.StaffActions.Update(action);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Action submitted.");
		}

		[Command("my actions")]
		public async Task MyActions()
		{
			var activeActions = _stewardContext.StaffActions.Where(sa => sa.SubmitterId == Context.User.Id.ToString());

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
				if (sa.AssignedToId == null)
				{
					var embedFieldBuilder = new EmbedFieldBuilder
					{
						Value = sa.ActionResponse == null ? "No response yet." : $"Response: {sa.ActionResponse}",
						Name = sa.ActionTitle + " - " + sa.Status.ToString(),
						IsInline = false
					};

					embedBuilder.AddField(embedFieldBuilder);
				}
				else
				{
					var staffAssignedTo = _client.GetUser(ulong.Parse(sa.AssignedToId));

					var embedFieldBuilder = new EmbedFieldBuilder
					{
						Value = sa.ActionResponse == null ? "No response yet." : $"Response: {sa.ActionResponse}",
						Name = sa.ActionTitle + " - " + sa.Status.ToString() + " - " + staffAssignedTo.Username,
						IsInline = false
					};

					embedBuilder.AddField(embedFieldBuilder);
				}
			}

			await ReplyAsync("", false, embedBuilder.Build(), null);
		}
	}
}
