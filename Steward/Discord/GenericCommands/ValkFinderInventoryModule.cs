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

        [Command("give")]
        public async Task addItem(string itemName, string type, [Remainder]SocketGuildUser mention)//add item to mentioned player 
        {
            //check wheter ItemID of type exists
            //check wheter player has item or is admin
            //give player item
            
            /*switch (type)
            {
                case "weapon":
                    item = _stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == itemName);
                    break;
                case "armour":
                    item = _stewardContext.ValkFinderArmours.FirstOrDefault(w => w.ArmourName == itemName);
                    break;
                case "item":
                    item = _stewardContext.ValkFinderItems.FirstOrDefault(w => w.ItemName == itemName);
                    break;
                default:
                    await ReplyAsync($"{type} is not a valid item type. Types are armour/weapon/item.");
                    return;
            }*/
        }
    }
}
