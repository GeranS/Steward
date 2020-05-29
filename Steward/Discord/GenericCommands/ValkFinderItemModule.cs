using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;


namespace Steward.Discord.GenericCommands
{
    public class ValkFinderItemModule : ModuleBase<SocketCommandContext>
    {
        private readonly StewardContext _stewardContext;

        public ValkFinderItemModule(StewardContext c)
        {
            _stewardContext = c;
        }


        [Command("add item")]
        [RequireStewardPermission]
        public async Task AddItem(string name, string description)
        {
            if (name.Length > 100)
            {
                await ReplyAsync("Item name can't be longer than 100 characters.");
                return;
            }
            if (description.Length > 500)
            {
                await ReplyAsync("Item description can't be longer than 500 characters.");
                return;
            }

            var newItem = new ValkFinderItem()
            {
                ItemName = name,
                ItemDescription = description
            };

            _stewardContext.ValkFinderItems.Add(newItem);
            await _stewardContext.SaveChangesAsync();

            await ReplyAsync("Item added.");
        }

        [Command("items")]
        [RequireStewardPermission]
        public async Task ListItems()
        {
            var embedBuilder = new EmbedBuilder();

            var valkFinderItems = _stewardContext.ValkFinderItems.ToList();

            var sortedValkFinderItems = valkFinderItems.OrderBy(v => v.ItemName);

            var stringBuilder = new StringBuilder();

            foreach (var item in sortedValkFinderItems)
            {
                stringBuilder.AppendLine($"{item.ItemName}, {item.ItemDescription}");
            }

            embedBuilder.AddField("Items", stringBuilder.ToString());

            await ReplyAsync(embed: embedBuilder.Build());
        }

    }
}
