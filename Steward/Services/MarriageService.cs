using Discord.WebSocket;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace Steward.Services
{
    public class MarriageService
    {
        private readonly DiscordSocketClient _client;

        private readonly StewardContext _stewardContext;

        public MarriageService(StewardContext stewardContext, DiscordSocketClient client)
        {
            _client = client;
            _stewardContext = stewardContext;
        }

        public async Task SendMarriageMessage(Proposal proposal)
        {
            var proposerID = proposal.Proposer.DiscordId;
            var proposedID = proposal.Proposed.DiscordId;
            var proposer = _client.GetUser(ulong.Parse(proposerID));
            var proposed = _client.GetUser(ulong.Parse(proposedID));
            var marriageChannels = _stewardContext.MarriageChannels.ToList();
            var ProposerChar = proposal.Proposer.Characters.FirstOrDefault(c => c.IsAlive());

            var ProposedChar = proposal.Proposed.Characters.FirstOrDefault(c => c.IsAlive());

            var embedBuilder = new EmbedBuilder()
            {
                Title = "A new noble marriage has happend!"
            };
            embedBuilder.AddField( new EmbedFieldBuilder()
            {
                Name = $"**{ProposerChar.CharacterName}**({proposer.ToString()})",
                Value = proposer.GetAvatarUrl(),
                IsInline = false
            });
            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = "married",
                Value = ":heart:",
                IsInline = true
            });
            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = $"**{ProposedChar.CharacterName}**({proposed.ToString()})",
                Value = proposed.GetAvatarUrl(),
                IsInline = true
            });
            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = ":heart",
                Value = "Aren't they cute together?"
            });

            foreach (var marriageChannel in marriageChannels.Select(marriage => _client.GetChannel(ulong.Parse(marriage.ChannelId)) as SocketTextChannel))
            {
                try
                {
                    await marriageChannel.SendMessageAsync(embed: embedBuilder.Build());
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.StackTrace);
                    //nothing, I just don't want it to crash the command
                }
            }
        }

        public async Task SendDivorceMessage(DiscordUser proposerUser, DiscordUser proposedUser)
        {
            var proposerID = proposerUser.DiscordId;
            var proposedID = proposedUser.DiscordId;
            var proposer = _client.GetUser(ulong.Parse(proposerID));
            var proposed = _client.GetUser(ulong.Parse(proposedID));
            var marriageChannels = _stewardContext.MarriageChannels.ToList();
            var ProposerChar = proposerUser.Characters.FirstOrDefault(c => c.IsAlive());

            var ProposedChar = proposedUser.Characters.FirstOrDefault(c => c.IsAlive());

            var embedBuilder = new EmbedBuilder()
            {
                Title = "Two spouses have decided to part forever!"
            };
            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = $"**{ProposerChar.CharacterName}**({proposer.ToString()})",
                Value = proposer.GetAvatarUrl(),
                IsInline = false
            });
            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = "and",
                Value = ":broken_heart:",
                IsInline = true
            });
            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = $"**{ProposedChar.CharacterName}**({proposed.ToString()})",
                Value = proposed.GetAvatarUrl(),
                IsInline = true
            });
            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = "have divorced!",
                Value = "How Sad!"
            });



            foreach (var marriageChannel in marriageChannels.Select(marriage => _client.GetChannel(ulong.Parse(marriage.ChannelId)) as SocketTextChannel))
            {
                try
                {
                    await marriageChannel.SendMessageAsync(embed: embedBuilder.Build());
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.StackTrace);
                    //nothing, I just don't want it to crash the command
                }
            }
        }
    }
}
