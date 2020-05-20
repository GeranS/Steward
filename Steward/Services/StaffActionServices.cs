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
	        var submitter = _client.GetUser(ulong.Parse(staffAction.SubmitterId));

            var embedBuilder = new EmbedBuilder
            {
                Color = Color.Purple,
                Title = staffAction.ActionTitle + " - " + staffAction.StaffActionId.ToString() + " - " + submitter.Username,
            };

            EmbedFieldBuilder embedFieldBuilder;

            if (staffAction.AssignedToId == null)
            {
	            embedFieldBuilder = new EmbedFieldBuilder
	            {
		            Value = staffAction.ActionDescription,
		            Name = staffAction.Status.ToString(),
		            IsInline = false
	            };
            }
            else
            {
	            var assignedStaff = _client.GetUser(ulong.Parse(staffAction.AssignedToId));

	            embedFieldBuilder = new EmbedFieldBuilder
	            {
		            Value = staffAction.ActionDescription,
		            Name = staffAction.Status.ToString() + " : " + assignedStaff.Username,
		            IsInline = false
	            };
            }

            embedBuilder.AddField(embedFieldBuilder);

            embedBuilder.AddField(new EmbedFieldBuilder
            {
                Name = "Response",
                Value = staffAction.ActionResponse == null ? "None." : staffAction.ActionResponse
            });

            return embedBuilder;
        }
    }
}
