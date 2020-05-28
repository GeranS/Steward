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

namespace Steward.Services
{
    public class InventoryService
    {
        StewardContext _stewardContext;

        public InventoryService(StewardContext s)
        {
            _stewardContext = s;
        }

        public async Task GiveItem(PlayerCharacter reciever, Object item, int amount, string type)
        {
            var recieverInv = _stewardContext.CharacterInventories.FirstOrDefault(i => i.PlayerCharacterId == reciever.CharacterId && (i.ValkFinderArmour == item || i.ValkFinderWeapon == item || i.ValkFinderItem == item));
            if (recieverInv == null)//he doesn't already have this type of item
            {
                recieverInv = new CharacterInventory()
                {
                    PlayerCharacterId = reciever.CharacterId,
                    Amount = amount
                };
                switch (type)
                {
                    case "weapon":
                        recieverInv.ValkFinderWeapon = item as ValkFinderWeapon;
                        break;
                    case "armour":
                        recieverInv.ValkFinderArmour = item as ValkFinderArmour;
                        break;
                    case "item":
                        recieverInv.ValkFinderItem = item as ValkFinderItem;
                        break;
                }
                _stewardContext.CharacterInventories.Add(recieverInv);

            }
            else //he already has this type of item
            {
                recieverInv.Amount += amount; //give him more
                _stewardContext.CharacterInventories.Update(recieverInv);
            }
            await _stewardContext.SaveChangesAsync();
        }

        public async Task TakeItem(PlayerCharacter victimChar, object item, int amount, string type)
        {
            var victimInv = _stewardContext.CharacterInventories.FirstOrDefault(i => i.PlayerCharacterId == victimChar.CharacterId && (i.ValkFinderArmour == item || i.ValkFinderWeapon == item || i.ValkFinderItem == item));
            victimInv.Amount -= amount;
            if (victimInv.Amount <= 0) //if its zero delete the inventory
            {
                _stewardContext.CharacterInventories.Remove(victimInv);
            }
            else //if its over zero update the inventory
            {
                _stewardContext.CharacterInventories.Update(victimInv);
            }
            await _stewardContext.SaveChangesAsync();
        }

        public EmbedBuilder createInventoryEmbed(PlayerCharacter character)
        {
            var characterInventory = _stewardContext.CharacterInventories.ToList().Where(i => i.PlayerCharacterId == character.CharacterId);
            var embedBuilder = new EmbedBuilder()
            {
                Color = Color.Purple,
                Title = "Inventory"
            };

            var weaponBuilder = new StringBuilder();
            var armourBuilder = new StringBuilder();
            var itemBuilder = new StringBuilder();

            foreach (var inv in characterInventory)
            {
                if (inv.ValkFinderArmour == null && inv.ValkFinderItem == null)
                {
                    weaponBuilder.AppendLine($"{inv.ValkFinderWeapon.WeaponName}: {inv.Amount}");
                }
                else if(inv.ValkFinderWeapon == null && inv.ValkFinderItem == null)
                {
                    armourBuilder.AppendLine($"{inv.ValkFinderArmour.ArmourName}: {inv.Amount}");
                }
                else if(inv.ValkFinderWeapon == null && inv.ValkFinderArmour == null)
                {
                    itemBuilder.AppendLine($"{inv.ValkFinderItem.ItemName}: {inv.Amount}");
                }

            }
            if (weaponBuilder != null)
            {
                embedBuilder.AddField("Weapons:", weaponBuilder.ToString());
            }
            if (armourBuilder != null)
            {
                embedBuilder.AddField("Armour:", armourBuilder.ToString());
            }
            if (itemBuilder != null)
            {
                embedBuilder.AddField("Items:", itemBuilder.ToString());
            }
            if (embedBuilder.Fields == null)
            {
                embedBuilder.AddField("Empty", "Your Inventory is empty!");
            }
            return embedBuilder;
        }

        public bool checkInv(PlayerCharacter character, string itemName, int amount)
        {
            //check for items in sender inv
            var senderInv = _stewardContext.CharacterInventories.FirstOrDefault(i => i.PlayerCharacterId == character.CharacterId && (i.ValkFinderArmour.ArmourName == itemName || i.ValkFinderWeapon.WeaponName == itemName || i.ValkFinderItem.ItemName == itemName));

            //does sender have enough items?
            if (senderInv == null || (senderInv.Amount <= amount))
            {
                
                return false;
            }
            return true;
        }

    }
}
