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
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
    public class ValkFinderInventoryModule : ModuleBase<SocketCommandContext>
    {
        private readonly StewardContext _stewardContext;
        private readonly InventoryService _inventoryService;
        public ValkFinderInventoryModule(StewardContext c, InventoryService i)
        {
            _inventoryService = i;
            _stewardContext = c;
        }

        [Command("add item")]
        [RequireStewardPermission]
        public async Task addItem(string ItemID, [Remainder]SocketGuildUser mention)//add item to mentioned player 
        {

        }
    }
}
