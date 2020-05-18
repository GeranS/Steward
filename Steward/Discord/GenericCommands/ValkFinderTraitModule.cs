using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
	public class ValkFinderTraitModule : ModuleBase<SocketCommandContext>
	{
		private readonly RollService _rollService;
		private readonly StewardContext _stewardContext;
		private readonly CharacterService _characterService;

		public ValkFinderTraitModule(StewardContext c, RollService r, CharacterService characterService)
		{
			_rollService = r;
			_stewardContext = c;
			_characterService = characterService;
		}

		[Command("add trait")]
		[Summary("Example: -1 0 0 1 0 \"Court Education - You have been educated in the writ of law, and the book of justice or whatever.\"")]
		[RequireStewardPermission]
		public async Task CreateTrait(int str, int end, int dex, int per, int intel, int ac, int ap, int hp, string description, bool secret)
		{
			var newTrait = new Trait()
			{
				STR = str,
				END = end,
				DEX = dex,
				PER = per,
				INT = intel,
				ArmorClassBonus = ac,
				AbilityPointBonus = ap,
				HealthPoolBonus = hp,
				Description = description,
				IsSecret = secret
			};

			await _stewardContext.Traits.AddAsync(newTrait);
			await _stewardContext.SaveChangesAsync();
			await ReplyAsync("Trait created.");
		}

		[Command("trait")]
		public async Task AddTraitToCharacter(string traitName, [Remainder]SocketGuildUser mention = null)
		{
			var trait = _stewardContext.Traits.FirstOrDefault(t => t.Description.StartsWith(traitName.ToLowerInvariant()));

			if (trait == null)
			{
				await ReplyAsync($"Could not find a trait with the name {traitName}.");
				return;
			}

			DiscordUser discordUser = null;

			PlayerCharacter activeCharacter = null;

			if (mention == null)
			{
				discordUser = _stewardContext.DiscordUsers
					.Include(du => du.Characters)
					.ThenInclude(c => c.CharacterTraits)
					.ThenInclude(ct => ct.Trait)
					.SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

				activeCharacter = discordUser.Characters.Find(c => c.IsAlive());

				if (activeCharacter == null)
				{
					await ReplyAsync("Could not find a living character.");
					return;
				}

				if (activeCharacter.CharacterTraits.Count > 0 && !discordUser.CanUseAdminCommands && mention == null)
				{
					await ReplyAsync(
						"You may only add one trait to yourself, if you need additional traits or need a trait removed, please contact an admin or Major GM.");
					return;
				}
			}
			else
			{
				discordUser = _stewardContext.DiscordUsers
					.Include(du => du.Characters)
					.ThenInclude(c => c.CharacterTraits)
					.ThenInclude(ct => ct.Trait)
					.SingleOrDefault(u => u.DiscordId == mention.Id.ToString());

				activeCharacter = discordUser.Characters.Find(c => c.IsAlive());

				if (activeCharacter == null)
				{
					await ReplyAsync("Could not find a living character.");
					return;
				}

				var commandUser =
					_stewardContext.DiscordUsers.SingleOrDefault(du => du.DiscordId == Context.User.Id.ToString());

				if (!commandUser.CanUseAdminCommands)
				{
					await ReplyAsync("You don't have the required permissions to use this command.");
					return;
				}
			}

			var traitAlreadyExistsList = activeCharacter.CharacterTraits.Where(ct => ct.Trait == trait);
			if (traitAlreadyExistsList.Count() != 0)
			{
				activeCharacter.CharacterTraits.Remove(traitAlreadyExistsList.First());
				_stewardContext.PlayerCharacters.Update(activeCharacter);
				await _stewardContext.SaveChangesAsync();
				await ReplyAsync("Trait has been removed.");
				return;
			}

			var newCharacterTrait = new CharacterTrait()
			{
				Trait = trait,
				PlayerCharacter = activeCharacter
			};

			await _stewardContext.CharacterTraits.AddAsync(newCharacterTrait);
			await _stewardContext.SaveChangesAsync();
			await ReplyAsync("Trait has been added.");
		}

		[Command("traits secret")]
		[RequireStewardPermission]
		public async Task ShowTraitsAdmin()
		{
			var traits = _stewardContext.Traits.Where(t => t.IsSecret == true);

			var embedBuilder = new EmbedBuilder()
				.WithColor(Color.Purple)
				.WithTitle("Secret Traits");

			foreach (var trait in traits)
			{
				var bonusString = "";

				if (trait.STR != 0)
				{
					bonusString += $"STR({trait.STR}) ";
				}
				if (trait.DEX != 0)
				{
					bonusString += $"DEX({trait.DEX}) ";
				}
				if (trait.END != 0)
				{
					bonusString += $"END({trait.END}) ";
				}
				if (trait.PER != 0)
				{
					bonusString += $"PER({trait.PER}) ";
				}
				if (trait.INT != 0)
				{
					bonusString += $"INT({trait.INT}) ";
				}
				if (trait.ArmorClassBonus != 0)
				{
					bonusString += $"AC({trait.ArmorClassBonus}) ";
				}
				if (trait.AbilityPointBonus != 0)
				{
					bonusString += $"AP({trait.AbilityPointBonus}) ";
				}
				if (trait.HealthPoolBonus != 0)
				{
					bonusString += $"HP({trait.HealthPoolBonus}) ";
				}

				embedBuilder.AddField(trait.Description, bonusString, false);
			}

			await ReplyAsync("", false, embedBuilder.Build(), null);
		}

		[Command("traits")]
		public async Task ShowTraits()
		{
			var traits = _stewardContext.Traits.Where(t => t.IsSecret == false);

			var embedBuilder = new EmbedBuilder()
				.WithColor(Color.Purple)
				.WithTitle("Traits");

			foreach (var trait in traits)
			{
				var bonusString = "";

				if (trait.STR != 0)
				{
					bonusString += $"STR({trait.STR}) ";
				}
				if (trait.DEX != 0)
				{
					bonusString += $"DEX({trait.DEX}) ";
				}
				if (trait.END != 0)
				{
					bonusString += $"END({trait.END}) ";
				}
				if (trait.PER != 0)
				{
					bonusString += $"PER({trait.PER}) ";
				}
				if (trait.INT != 0)
				{
					bonusString += $"INT({trait.INT}) ";
				}
				if (trait.ArmorClassBonus != 0)
				{
					bonusString += $"AC({trait.ArmorClassBonus}) ";
				}
				if (trait.AbilityPointBonus != 0)
				{
					bonusString += $"AP({trait.AbilityPointBonus}) ";
				}
				if (trait.HealthPoolBonus != 0)
				{
					bonusString += $"HP({trait.HealthPoolBonus}) ";
				}

				embedBuilder.AddField(trait.Description, bonusString, false);
			}

			await ReplyAsync("", false, embedBuilder.Build(), null);
		}

		[Command("change trait stats")]
		[RequireStewardPermission]
		public async Task ChangeTraitStat(string traitName, int str, int end, int dex, int per, int intel, int ac, int ap, int hp)
		{
			var trait = _stewardContext.Traits.FirstOrDefault(t => t.Description.StartsWith(traitName.ToLowerInvariant()));

			if (trait == null)
			{
				await ReplyAsync($"Could not find a trait with the name {traitName}.");
				return;
			}

			trait.STR = str;
			trait.END = end;
			trait.DEX = dex;
			trait.PER = per;
			trait.INT = intel;
			trait.ArmorClassBonus = ac;
			trait.AbilityPointBonus = ap;
			trait.HealthPoolBonus = hp;

			_stewardContext.Traits.Update(trait);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Stats of Trait Changed!");
		}

		[Command("change trait desc")]
		[RequireStewardPermission]
		public async Task ChangeTraitDesc(string traitName, string description)
		{
			var trait = _stewardContext.Traits.FirstOrDefault(t => t.Description.StartsWith(traitName.ToLowerInvariant()));

			if (trait == null)
			{
				await ReplyAsync($"Could not find a trait with the name {traitName}.");
				return;
			}

			trait.Description = description;

			_stewardContext.Traits.Update(trait);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Description of Trait Changed!");
		}
	}
}
