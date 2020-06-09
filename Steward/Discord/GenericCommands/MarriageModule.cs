using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
	public class MarriageModule : ModuleBase<SocketCommandContext>
	{
		private readonly StewardContext _stewardContext;
		private readonly DiscordSocketClient _client;
		private readonly MarriageService _marriageService;

		public MarriageModule(StewardContext context, DiscordSocketClient client, MarriageService m)
		{
			_stewardContext = context;
			_client = client;
			_marriageService = m;
		}

		[Command("propose")]
		public async Task MarryPlayer([Remainder]SocketGuildUser mention)
		{
			var commandUser = _stewardContext.DiscordUsers
					.Include(du => du.Characters)
					.SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

			var activeCharacter = commandUser.Characters.Find(c => c.IsAlive());

			if (activeCharacter == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}
			var discordUser = _stewardContext.DiscordUsers
					.Include(du => du.Characters)
					.SingleOrDefault(u => u.DiscordId == mention.Id.ToString());

			var receivingCharacter = discordUser.Characters.Find(c => c.IsAlive());

			if (receivingCharacter.CharacterId == activeCharacter.CharacterId)
            {
				await ReplyAsync("You can't propose to yourself even if you love yourself soooo much!");
				return;
            }

			if (receivingCharacter == null)
			{
				await ReplyAsync($"Could not find a living character for {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}.");
				return;
			}

			var activeSpouse = _stewardContext.PlayerCharacters.FirstOrDefault(c => c.CharacterId == activeCharacter.SpouseId);

			if (activeSpouse != null && activeSpouse.IsAlive())
			{
				await ReplyAsync("You are already married!");
				return;
			}

			var receiverSpouse = _stewardContext.PlayerCharacters.FirstOrDefault(c => c.CharacterId == receivingCharacter.SpouseId);

			if (receiverSpouse != null && receiverSpouse.IsAlive())
			{
				await ReplyAsync($"{receivingCharacter.CharacterName} is already married. Kill their spouse first to marry her/him");
				return;
			}

			if (_stewardContext.Proposals.FirstOrDefault(c => c.ProposerId == activeCharacter.DiscordUser.DiscordId && c.ProposedId == receivingCharacter.DiscordUser.DiscordId) != null)
			{
				await ReplyAsync($"You have already proposed to this Character!");
				return;
			}

			if (_stewardContext.Proposals.FirstOrDefault(c => c.ProposedId == activeCharacter.CharacterId && c.ProposerId == receivingCharacter.CharacterId) != null)
			{
				await ReplyAsync($"This Character has already proposed to you, reply to their advances first. To find the proposal check your DMs");
				return;
			}

			/*activeCharacter.SpouseId = receivingCharacter.CharacterId;
            receivingCharacter.SpouseId = activeCharacter.CharacterId;

            await _stewardContext.SaveChangesAsync();*/

			var newProposal = new Proposal()
			{
				Proposer = commandUser.Characters.SingleOrDefault(cu => cu.IsAlive()),
				Proposed = discordUser.Characters.SingleOrDefault(du => du.IsAlive())
			};

			_stewardContext.Proposals.Add(newProposal);
			await _stewardContext.SaveChangesAsync();

			//PM player to notify he has been proposed to.

			var proposed = _client.GetUser(ulong.Parse(discordUser.DiscordId));

			var embedBuilder = new EmbedBuilder
			{
				Color = Color.Purple,
				Title = $"A suitor has approached you! (ProposalID = {newProposal.ProposalId})",
			};

			embedBuilder.AddField(new EmbedFieldBuilder
			{
				Name = "Proposer:",
				Value = $"{activeCharacter.CharacterName} has asked for your hand in marriage."
			});

			embedBuilder.AddField(new EmbedFieldBuilder
			{
				Name = "Accept or Deny",
				Value = $"Use \"&accept {newProposal.ProposalId}\" or \"&deny {newProposal.ProposalId}\" to respond."
			});

			try
			{
				await proposed.SendMessageAsync("", false, embedBuilder.Build());
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
				//nothing, I just don't want it to crash the command
			}

			await ReplyAsync($"You have proposed to {receivingCharacter.CharacterName}!");
		}


		[Command("accept")]
		[RequireActiveCharacter]
		public async Task AcceptProposal(int ProposalId)
		{
			var commandUser = _stewardContext.DiscordUsers
					.Include(du => du.Characters)
					.SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

			var activeCharacter = commandUser.Characters.SingleOrDefault(cu => cu.IsAlive());

			var proposal = _stewardContext.Proposals
				.Include(p => p.Proposed)
				.Include(p => p.Proposer)
				.FirstOrDefault(p => p.ProposalId == ProposalId);

			if (proposal == null)
			{
				await ReplyAsync($"No Proposal with ID {ProposalId} found.");
				return;
			}

			var proposerChar = proposal.Proposer;

			var proposedChar = proposal.Proposed;

			if (activeCharacter.CharacterId != proposal.ProposedId)
			{
				await ReplyAsync("You can only reply to proposals directed at you.");
				return;
			}

			if (!proposerChar.IsAlive())
			{
				await ReplyAsync("No living Character has been found for the Proposer. He might have died already.");
				return;
			}
			if (!proposedChar.IsAlive())
			{
				await ReplyAsync("Could not find living Character.");
				return;
			}

			if (proposerChar.SpouseId != null && _stewardContext.PlayerCharacters.FirstOrDefault(p => p.SpouseId == proposerChar.CharacterId && p.YearOfDeath == null) != null)
			{
				await ReplyAsync("Your suitor has already married another one. You are too late.");
				return;
			}

			if (proposedChar.SpouseId != null && _stewardContext.PlayerCharacters.FirstOrDefault(p => p.SpouseId == proposedChar.CharacterId && p.YearOfDeath == null) != null)
			{
				await ReplyAsync("You are already married! Kill your spouse first to accept this proposal.");
				return;
			}

			proposerChar.SpouseId = proposedChar.CharacterId;
			proposedChar.SpouseId = proposerChar.CharacterId;

			try
			{
				await _client.GetUser(ulong.Parse(proposal.Proposer.DiscordUserId)).SendMessageAsync($"Congrats {proposedChar.CharacterName} has accepted your proposal for marriage! You are now officially married!");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
				//nothing, I just don't want it to crash the command
			}

			await _marriageService.SendMarriageMessage(proposal);

			_stewardContext.Proposals.Remove(proposal);
			await _stewardContext.SaveChangesAsync();
			await ReplyAsync("Congratulations to your new marriage!");

		}

		[Command("deny")]
		[RequireActiveCharacter]
		public async Task DenyProposal(int ProposalID)
		{
			var commandUser = _stewardContext.DiscordUsers
					.Include(du => du.Characters)
					.SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

			var activeCharacter = commandUser.Characters.SingleOrDefault(cu => cu.IsAlive());

			var proposal = _stewardContext.Proposals
				.Include(p => p.Proposed)
				.Include(p => p.Proposer)
				.FirstOrDefault(p => p.ProposalId == ProposalID);

			if (proposal == null)
			{
				await ReplyAsync($"No Proposal with ID {ProposalID} found.");
				return;
			}

			if (activeCharacter.CharacterId != proposal.ProposedId)
			{
				await ReplyAsync("You can only reply to proposals directed at you.");
				return;
			}

			await ReplyAsync($"You have rejected the proposal!");
			try
			{
				await _client.GetUser(ulong.Parse(proposal.Proposer.DiscordUserId)).SendMessageAsync($"Your proposal has been rejected!");
			}
			catch (NullReferenceException e)
			{
				Console.WriteLine(e.StackTrace);
				//nothing, I just don't want it to crash the command
			}

			_stewardContext.Proposals.Remove(proposal);
			await _stewardContext.SaveChangesAsync();
		}

		[Command("set marriage channel")]
		[RequireStewardPermission]
		public async Task SetMarriageChannel()
		{
			var existingMarriageChannel = _stewardContext.MarriageChannels.ToList();

			if (existingMarriageChannel.Count != 0)
			{
				await ReplyAsync("There's already an existing StaffAction channel.");
				return;
			}

			var MarriageChannel = new MarriageChannel()
			{
				ChannelId = Context.Channel.Id.ToString(),
				ServerId = Context.Guild.Id.ToString()
			};

			await _stewardContext.MarriageChannels.AddAsync(MarriageChannel);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Marriage Channel added!");
		}

		[Command("divorce")]
		[RequireStewardPermission]
		public async Task Divorce(SocketGuildUser mention1, SocketGuildUser mention2)
		{
			var spouse1 = _stewardContext.DiscordUsers
					.Include(du => du.Characters)
					.SingleOrDefault(u => u.DiscordId == mention1.Id.ToString());
			var spouse2 = _stewardContext.DiscordUsers
					.Include(du => du.Characters)
					.SingleOrDefault(u => u.DiscordId == mention2.Id.ToString());

			var spouse1Char = spouse1.Characters.Find(c => c.IsAlive());
			var spouse2Char = spouse2.Characters.Find(c => c.IsAlive());

			if (spouse1Char == null || spouse2Char == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			if (spouse1Char.SpouseId != spouse2Char.CharacterId)
			{
				await ReplyAsync("The mentioned players do not have living married characters.");
				return;
			}

			spouse1Char.SpouseId = null;
			spouse2Char.SpouseId = null;

			await _marriageService.SendDivorceMessage(spouse1, spouse2);
			await _stewardContext.SaveChangesAsync();
			await ReplyAsync("A divorce has happened, how sad.");
		}

	}
}
