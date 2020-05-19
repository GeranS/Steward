using Discord;
using Discord.WebSocket;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Steward.Services
{
    public class StaffActionService
    {
        private readonly StewardContext _stewardContext;
        private readonly DiscordSocketClient _client;

        public StaffActionService(StewardContext stewardContext, DiscordSocketClient client)
        {
            _stewardContext = stewardContext;
            _client = client;
        }


        public EmbedBuilder BuildStaffActionMessage(StaffAction staffAction)
        {
            var embedBuilder = new EmbedBuilder
            {
                Color = Color.Purple,
                Title = staffAction.ActionTitle,
            };

            var assignedStaff = _client.GetUser(ulong.Parse(staffAction.AssignedToId));

            var embedFieldBuilder = new EmbedFieldBuilder
            {
                Value = staffAction.ActionDescription,
                Name = staffAction.Status.ToString()+ " : "+assignedStaff.Mention,
                IsInline = false
            };

            embedBuilder.AddField(embedFieldBuilder);

            embedBuilder.AddField(new EmbedFieldBuilder
            {
                Name = "Response",
                Value = staffAction.ActionResponse
            });

            return embedBuilder;
        }
    }
}
