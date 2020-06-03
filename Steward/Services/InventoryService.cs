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
		private readonly StewardContext _stewardContext;

		public InventoryService(StewardContext s)
		{
			_stewardContext = s;
		}

		public async Task<string> GiveItem(PlayerCharacter receiverOld, string name, string type, int amount)
		{
			var receiver = _stewardContext.PlayerCharacters
				.Include(pc => pc.DefaultMeleeWeapon)
				.Include(pc => pc.DefaultRangedWeapon)
				.Include(pc => pc.CharacterInventories)
				.ThenInclude(i => i.ValkFinderWeapon)
				.Include(pc => pc.CharacterInventories)
				.ThenInclude(i => i.ValkFinderArmour)
				.Include(pc => pc.CharacterInventories)
				.ThenInclude(i => i.ValkFinderItem)
				.FirstOrDefault(pc => pc.CharacterId == receiverOld.CharacterId);
			switch (type)
			{
				case "weapon":
					var weapon = _stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == name);
					if (weapon == null)
					{
						return "Could not find weapon.";
					}

					if (!receiver.CharacterInventories.Any(i =>
						i.ValkFinderWeaponId != null && i.ValkFinderWeaponId == weapon.ValkFinderWeaponId))
					{
						var newInventory = new CharacterInventory()
						{
							PlayerCharacterId = receiver.CharacterId,
							Amount = amount,
							ValkFinderWeapon = weapon
						};

						_stewardContext.CharacterInventories.Add(newInventory);
					}
					else
					{
						var inv = _stewardContext.CharacterInventories.FirstOrDefault(i =>
							i.PlayerCharacterId == receiver.CharacterId &&
							i.ValkFinderWeaponId == weapon.ValkFinderWeaponId);

						inv.Amount += amount;
					}

					await _stewardContext.SaveChangesAsync();
					return null;
				case "armour":
					var armour = _stewardContext.ValkFinderArmours.FirstOrDefault(w => w.ArmourName == name);
					if (armour == null)
					{
						return "Could not find armour.";
					}
					if (!receiver.CharacterInventories.Any(i => i.ValkFinderArmour != null && i.ValkFinderArmourId == armour.ValkFinderArmourId))
					{
						var newInventory = new CharacterInventory()
						{
							PlayerCharacterId = receiver.CharacterId,
							Amount = amount,
							ValkFinderArmour = armour
						};

						_stewardContext.CharacterInventories.Add(newInventory);
					}
					else
					{
						var inv = _stewardContext.CharacterInventories.FirstOrDefault(i =>
							i.PlayerCharacterId == receiver.CharacterId &&
							i.ValkFinderArmourId == armour.ValkFinderArmourId);

						inv.Amount += amount;
					}

					await _stewardContext.SaveChangesAsync();
					return null;
				case "item":
					var item = _stewardContext.ValkFinderItems.FirstOrDefault(w => w.ItemName == name);
					if (item == null)
					{
						return "Could not find item.";
					}
					if (!receiver.CharacterInventories.Any(i => i.ValkFinderItem != null && i.ValkFinderItem == item))
					{
						var newInventory = new CharacterInventory()
						{
							PlayerCharacterId = receiver.CharacterId,
							Amount = amount,
							ValkFinderItem = item
						};

						_stewardContext.CharacterInventories.Add(newInventory);
					}
					else
					{
						var inv = _stewardContext.CharacterInventories.FirstOrDefault(i =>
							i.PlayerCharacterId == receiver.CharacterId &&
							i.ValkFinderItemId == item.ValkFinderItemId);

						inv.Amount += amount;
					}

					await _stewardContext.SaveChangesAsync();
					return null;
			}

			return "Invalid type.";
		}

		/// <summary>
		/// Returns null on success
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		/// <returns>string</returns>
		public async Task<string> TakeItem(PlayerCharacter receiverOld, string name, string type, int amount)
		{
			var receiver = _stewardContext.PlayerCharacters
				.Include(pc => pc.DefaultMeleeWeapon)
				.Include(pc => pc.DefaultRangedWeapon)
				.Include(pc => pc.CharacterInventories)
				.ThenInclude(i => i.ValkFinderWeapon)
				.Include(pc => pc.CharacterInventories)
				.ThenInclude(i => i.ValkFinderArmour)
				.Include(pc => pc.CharacterInventories)
				.ThenInclude(i => i.ValkFinderItem)
				.FirstOrDefault(pc => pc.CharacterId == receiverOld.CharacterId);
			switch (type)
			{
				case "weapon":
					var weapon = _stewardContext.ValkFinderWeapons.FirstOrDefault(w => w.WeaponName == name);
					if (weapon == null)
					{
						return "Weapon does not exist.";
					}

					var inventoryWeapon =
						receiver.CharacterInventories.FirstOrDefault(i => i.ValkFinderWeaponId == weapon.ValkFinderWeaponId);

					if (inventoryWeapon == null)
					{
						return "Could not find weapon in inventory.";
					}

					if (amount >= inventoryWeapon.Amount)
					{
						receiver.CharacterInventories.Remove(inventoryWeapon);

						if (receiver.DefaultMeleeWeaponId == weapon.ValkFinderWeaponId)
						{
							receiver.DefaultMeleeWeaponId = null;
						}

						if (receiver.DefaultRangedWeaponId == weapon.ValkFinderWeaponId)
						{
							receiver.DefaultRangedWeaponId = null;
						}
					}
					else
					{
						var inv = _stewardContext.CharacterInventories.FirstOrDefault(i =>
							i.PlayerCharacterId == receiver.CharacterId &&
							i.ValkFinderWeaponId == weapon.ValkFinderWeaponId);

						inv.Amount -= amount;
					}

					await _stewardContext.SaveChangesAsync();
					return null;
				case "armour":
					var armour = _stewardContext.ValkFinderArmours.FirstOrDefault(w => w.ArmourName == name);
					if (armour == null)
					{
						return "Armour does not exist.";
					}

					var inventoryArmour =
						receiver.CharacterInventories.FirstOrDefault(i => i.ValkFinderArmourId == armour.ValkFinderArmourId);

					if (inventoryArmour == null)
					{
						return "Could not find armour in inventory.";
					}

					if (amount >= inventoryArmour.Amount)
					{
						receiver.CharacterInventories.Remove(inventoryArmour);

						if (receiver.EquippedArmourId == armour.ValkFinderArmourId)
						{
							receiver.EquippedArmourId = null;
						}
					}
					else
					{
						var inv = _stewardContext.CharacterInventories.FirstOrDefault(i =>
							i.PlayerCharacterId == receiver.CharacterId &&
							i.ValkFinderArmourId == armour.ValkFinderArmourId);

						inv.Amount -= amount;
					}

					await _stewardContext.SaveChangesAsync();
					return null;
				case "item":
					var item = _stewardContext.ValkFinderItems.FirstOrDefault(w => w.ItemName == name);
					if (item == null)
					{
						return "Item does not exist.";
					}

					var inventoryItem =
						receiver.CharacterInventories.FirstOrDefault(i => i.ValkFinderItemId == item.ValkFinderItemId);

					if (inventoryItem == null)
					{
						return "Could not find item in inventory.";
					}

					if (amount >= inventoryItem.Amount)
					{
						receiver.CharacterInventories.Remove(inventoryItem);
					}
					else
					{
						var inv = _stewardContext.CharacterInventories.FirstOrDefault(i =>
							i.PlayerCharacterId == receiver.CharacterId &&
							i.ValkFinderItemId == item.ValkFinderItemId);

						inv.Amount -= amount;
					}

					await _stewardContext.SaveChangesAsync();
					return null;
			}

			return "Invalid type.";
		}

		public EmbedBuilder CreateInventoryEmbed(PlayerCharacter character)
		{
			var characterInventory = character.CharacterInventories;
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
				else if (inv.ValkFinderWeapon == null && inv.ValkFinderItem == null)
				{
					armourBuilder.AppendLine($"{inv.ValkFinderArmour.ArmourName}: {inv.Amount}");
				}
				else if (inv.ValkFinderWeapon == null && inv.ValkFinderArmour == null)
				{
					itemBuilder.AppendLine($"{inv.ValkFinderItem.ItemName}: {inv.Amount}");
				}

			}
			if (weaponBuilder.Length != 0)
			{
				embedBuilder.AddField("Weapons:", weaponBuilder.ToString());
			}
			if (armourBuilder.Length != 0)
			{
				embedBuilder.AddField("Armour:", armourBuilder.ToString());
			}
			if (itemBuilder.Length != 0)
			{
				embedBuilder.AddField("Items:", itemBuilder.ToString());
			}
			if (embedBuilder.Fields.Count == 0)
			{
				embedBuilder.AddField("Empty", "Your Inventory is empty!");
			}
			return embedBuilder;
		}

		public bool CheckInv(PlayerCharacter character, string itemName, string type, int amount)
		{
			switch (type)
			{
				case "weapon":
					var senderInvWeapon = _stewardContext.CharacterInventories.FirstOrDefault(i => i.PlayerCharacterId == character.CharacterId && i.ValkFinderWeapon.WeaponName == itemName);
					return senderInvWeapon != null && (senderInvWeapon.Amount >= amount);
				case "armour":
					var senderInvArmour = _stewardContext.CharacterInventories.FirstOrDefault(i => i.PlayerCharacterId == character.CharacterId && i.ValkFinderArmour.ArmourName == itemName);
					return senderInvArmour != null && (senderInvArmour.Amount >= amount);
				case "item":
					var senderInvItem = _stewardContext.CharacterInventories.FirstOrDefault(i => i.PlayerCharacterId == character.CharacterId && i.ValkFinderItem.ItemName == itemName);
					return senderInvItem != null && (senderInvItem.Amount >= amount);
			}

			return false;
		}

	}
}
