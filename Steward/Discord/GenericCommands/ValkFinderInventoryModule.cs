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
    public class ValkFinderInventoryModule : ModuleBase<SocketCommandContext>
    {
        private readonly StewardContext _stewardContext;
        private readonly InventoryService _inventoryService;
        private readonly DiscordSocketClient _client;
        public ValkFinderInventoryModule(StewardContext c, InventoryService i, DiscordSocketClient cl)
        {
            _inventoryService = i;
            _stewardContext = c;
            _client = cl;
        }

        [Command("give")]
        public async Task transfer(string itemName, string type, [Remainder]SocketGuildUser mention, int amount = 1)//add item to mentioned player 
        {
            //check wheter ItemID of type exists
            //check wheter player has item 
            //give player item
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

            var recievingCharacter = discordUser.Characters.Find(c => c.IsAlive());

            if (recievingCharacter == null)
            {
                await ReplyAsync($"Could not find a living character for {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}.");
                return;
            }
            object item;
            switch (type)
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
            }
            if (item == null)
            {
                await ReplyAsync($"{itemName} either does not exist or is not a {type}");
                return;
            }

            //check for items in sender inv
            var senderInv = _stewardContext.CharacterInventories.FirstOrDefault(i => i.PlayerCharacterId == activeCharacter.CharacterId && (i.ValkFinderArmour == item || i.ValkFinderWeapon == item || i.ValkFinderItem == item));

            //does sender have enough items?
            if (senderInv == null || (senderInv.Amount <= amount))
            {
                await ReplyAsync($"You don't have enough of {itemName} in you inventory to give {amount} away");
                return;
            }

            await _inventoryService.GiveItem(recievingCharacter, item, amount, type); //gives item to reciever

            await _inventoryService.TakeItem(senderInv, item, amount, type); //takes item with sender inventory

            await ReplyAsync($"{amount} of the {type} {itemName} has been given to {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}");
        }

        [Command("inventory")]
        public async Task showInventory([Remainder]SocketGuildUser mention = null)
        {
            DiscordUser user = null;

            if (mention != null)
            {
                user = _stewardContext.DiscordUsers
                    .Include(du => du.Characters)
                    .SingleOrDefault(u => u.DiscordId == mention.Id.ToString());
            }
            else
            {
                user = _stewardContext.DiscordUsers
                    .Include(du => du.Characters)
                    .SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());
            }

            var activeCharacter = user.Characters.FirstOrDefault(c => c.IsAlive());

            if (activeCharacter == null)
            {
                await ReplyAsync("Could not find a living character.");
                return;
            }

            var embedBuilder = _inventoryService.createInventory(activeCharacter);
            await ReplyAsync(embed: embedBuilder.Build());
            
        }

        [Command("grant item")]
        [RequireStewardPermission]
        public async Task grantItem(string itemName, string type, [Remainder]SocketGuildUser mention, int amount = 1)
        {
            var discordUser = _stewardContext.DiscordUsers
                    .Include(du => du.Characters)
                    .SingleOrDefault(u => u.DiscordId == mention.Id.ToString());

            var recievingCharacter = discordUser.Characters.Find(c => c.IsAlive());

            if (recievingCharacter == null)
            {
                await ReplyAsync($"Could not find a living character for {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}.");
                return;
            }
            object item;
            switch (type)
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
            }
            if (item == null)
            {
                await ReplyAsync($"{itemName} either does not exist or is not a {type}");
                return;
            }

            await _inventoryService.GiveItem(recievingCharacter, item, amount, type);
            await ReplyAsync($"{amount} of the {type} {itemName} has been granted to {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}");
        }

        [Command("take item")]
        [RequireStewardPermission]
        public async Task takeItem(string itemName, string type, [Remainder]SocketGuildUser mention, int amount = 1)
        {
            var discordUser = _stewardContext.DiscordUsers
                    .Include(du => du.Characters)
                    .SingleOrDefault(u => u.DiscordId == mention.Id.ToString());

            var victimCharacter = discordUser.Characters.Find(c => c.IsAlive());

            if (victimCharacter == null)
            {
                await ReplyAsync($"Could not find a living character for {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}.");
                return;
            }
            object item;
            switch (type)
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
            }
            if (item == null)
            {
                await ReplyAsync($"{itemName} either does not exist or is not a {type}");
                return;
            }
            var victimInv = _stewardContext.CharacterInventories.FirstOrDefault(i => i.PlayerCharacterId == victimCharacter.CharacterId && (i.ValkFinderArmour == item || i.ValkFinderWeapon == item || i.ValkFinderItem == item));

            await _inventoryService.TakeItem(victimInv, item, amount, type);
            await ReplyAsync($"{amount} of the {type} {itemName} has been taken from {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}");
        }
    }
}
