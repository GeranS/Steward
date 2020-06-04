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
using Newtonsoft.Json.Serialization;
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
        public async Task Transfer(string itemName, string type, SocketGuildUser mention, int amount = 1)//add item to mentioned player 
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

            var receivingCharacter = discordUser.Characters.Find(c => c.IsAlive());

            if (receivingCharacter == null)
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

            //does sender have enough items?
            if (!_inventoryService.CheckInv(activeCharacter, itemName, type, amount))
            {
                await ReplyAsync($"You don't have enough of {itemName} in you inventory to give {amount} away.");
                return;
            }

            await _inventoryService.GiveItem(receivingCharacter, itemName, type, amount); //gives item to reciever

            await _inventoryService.TakeItem(activeCharacter, itemName, type, amount); //takes item with sender inventory

            await ReplyAsync($"{amount} of the {type} {itemName} has been given to {receivingCharacter.CharacterName}");
        }

        [Command("inventory")]
        public async Task ShowInventory([Remainder]SocketGuildUser mention = null)
        {
            DiscordUser user = null;

            if (mention != null)
            {
                user = _stewardContext.DiscordUsers
	                .Include(du => du.Characters)
	                .ThenInclude(ch => ch.CharacterInventories)
	                .ThenInclude(ci => ci.ValkFinderWeapon)
	                .Include(du => du.Characters)
	                .ThenInclude(ch => ch.CharacterInventories)
	                .ThenInclude(ci => ci.ValkFinderArmour)
	                .Include(du => du.Characters)
	                .ThenInclude(ch => ch.CharacterInventories)
	                .ThenInclude(ci => ci.ValkFinderItem)
                    .SingleOrDefault(u => u.DiscordId == mention.Id.ToString());
            }
            else
            {
                user = _stewardContext.DiscordUsers
	                .Include(du => du.Characters)
	                .ThenInclude(ch => ch.CharacterInventories)
	                .ThenInclude(ci => ci.ValkFinderWeapon)
	                .Include(du => du.Characters)
	                .ThenInclude(ch => ch.CharacterInventories)
	                .ThenInclude(ci => ci.ValkFinderArmour)
	                .Include(du => du.Characters)
	                .ThenInclude(ch => ch.CharacterInventories)
	                .ThenInclude(ci => ci.ValkFinderItem)
                    .SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());
            }

            var activeCharacter = user.Characters.FirstOrDefault(c => c.IsAlive());

            if (activeCharacter == null)
            {
                await ReplyAsync("Could not find a living character.");
                return;
            }

            var embedBuilder = _inventoryService.CreateInventoryEmbed(activeCharacter);
            await ReplyAsync(embed: embedBuilder.Build());
            
        }

        [Command("grant item")]
        [RequireStewardPermission]
        public async Task GrantItem(string itemName, string type, SocketGuildUser mention, int amount = 1)
        {
	        var types = new[] {"weapon", "armour", "item"};

	        if (!types.Contains(type))
	        {
		        await ReplyAsync($"Type {type} does not exist.");
	        }

            var discordUser = _stewardContext.DiscordUsers
                    .Include(du => du.Characters)
                    .ThenInclude(ch => ch.CharacterInventories)
                    .ThenInclude(ci => ci.ValkFinderWeapon)
                    .Include(du => du.Characters)
                    .ThenInclude(ch => ch.CharacterInventories)
                    .ThenInclude(ci => ci.ValkFinderArmour)
                    .Include(du => du.Characters)
                    .ThenInclude(ch => ch.CharacterInventories)
                    .ThenInclude(ci => ci.ValkFinderItem)
                    .SingleOrDefault(u => u.DiscordId == mention.Id.ToString());

            var receivingCharacter = discordUser.Characters.Find(c => c.IsAlive());

            if (receivingCharacter == null)
            {
                await ReplyAsync($"Could not find a living character for {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}.");
                return;
            }
            
            var result = await _inventoryService.GiveItem(receivingCharacter, itemName, type, amount);

            if (result != null)
            {
	            await ReplyAsync(result);
	            return;
            }

            await ReplyAsync($"{amount} of the {type} {itemName} has been granted to {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}");
        }

        [Command("take item")]
        [RequireStewardPermission]
        public async Task TakeItem(string itemName, string type, SocketGuildUser mention, int amount = 1)
        {
	        var types = new[] { "weapon", "armour", "item" };

	        if (!types.Contains(type))
	        {
		        await ReplyAsync($"Type {type} does not exist.");
	        }

	        var discordUser = _stewardContext.DiscordUsers
		        .Include(du => du.Characters)
		        .ThenInclude(ch => ch.CharacterInventories)
		        .ThenInclude(ci => ci.ValkFinderWeapon)
		        .Include(du => du.Characters)
		        .ThenInclude(ch => ch.CharacterInventories)
		        .ThenInclude(ci => ci.ValkFinderArmour)
		        .Include(du => du.Characters)
		        .ThenInclude(ch => ch.CharacterInventories)
		        .ThenInclude(ci => ci.ValkFinderItem)
		        .SingleOrDefault(u => u.DiscordId == mention.Id.ToString());

	        var receivingCharacter = discordUser.Characters.Find(c => c.IsAlive());

	        if (receivingCharacter == null)
	        {
		        await ReplyAsync($"Could not find a living character for {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}.");
		        return;
	        }

	        var result = await _inventoryService.TakeItem(receivingCharacter, itemName, type, amount);

	        if (result != null)
	        {
		        await ReplyAsync(result);
		        return;
	        }

            await ReplyAsync($"{amount} of the {type} {itemName} has been taken from {_client.GetUser(ulong.Parse(discordUser.DiscordId)).ToString()}");
        }

        [Command("starting equipment")]
        [RequireActiveCharacter]
        public async Task StartingEquipment(string melee, string ranged, string armour)
        {
	        var discordUser = _stewardContext.DiscordUsers
                    .Include(du => du.Characters)
                    .ThenInclude(ch => ch.CharacterInventories)
                    .ThenInclude(ci => ci.ValkFinderWeapon)
                    .Include(du => du.Characters)
                    .ThenInclude(ch => ch.CharacterInventories)
                    .ThenInclude(ci => ci.ValkFinderArmour)
                    .Include(du => du.Characters)
                    .ThenInclude(ch => ch.CharacterInventories)
                    .ThenInclude(ci => ci.ValkFinderItem)
                    .SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

            var receivingCharacter = discordUser.Characters.Find(c => c.IsAlive());

            if (receivingCharacter.HasStartingEquipment == true)
            {
                await ReplyAsync($"You already have your starting equipment.");
                return;
            }

            if (_stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == melee) == null || _stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == ranged) == null || _stewardContext.ValkFinderArmours.FirstOrDefault(w => w.ArmourName == armour) == null)
            {
                await ReplyAsync("One of the items is not a valid item.");
                return;
            }

            var resultMelee = await _inventoryService.GiveItem(receivingCharacter, melee, "weapon", 1);
            var resultRanged = await _inventoryService.GiveItem(receivingCharacter, ranged, "weapon", 1);
            var resultArmour = await _inventoryService.GiveItem(receivingCharacter, armour, "armour", 1);

            if (resultMelee != null || resultRanged != null || resultArmour != null)
            {
                await ReplyAsync(resultMelee + resultArmour + resultRanged);
                return;
            }

            receivingCharacter.HasStartingEquipment = true;
            await _stewardContext.SaveChangesAsync();
            await ReplyAsync($"One of {melee}, {ranged}, and {armour} has been granted to {receivingCharacter.CharacterName}");
        }
    }
}
