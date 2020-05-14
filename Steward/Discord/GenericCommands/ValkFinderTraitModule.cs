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
		public async Task CreateTrait(int str, int end, int dex, int per, int intel, int ac, int ap, int hp, string description)
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
				Description = description
			};

			await _stewardContext.Traits.AddAsync(newTrait);
			await _stewardContext.SaveChangesAsync();
			await ReplyAsync("Trait created.");
		}

		[Command("trait")]
		[RequireStewardPermission]
		public async Task AddTraitToCharacter(string traitName, [Remainder]SocketGuildUser mention)
		{
			var trait = _stewardContext.Traits.FirstOrDefault(t => t.Description.StartsWith(traitName.ToLowerInvariant()));

			if (trait == null)
			{
				await ReplyAsync($"Could not find a trait with the name {trait}.");
				return;
			}

			var discordUser = _stewardContext.DiscordUsers
				.Include(du => du.Characters)
				.ThenInclude(c => c.CharacterTraits)
				.SingleOrDefault(u => u.DiscordId == mention.Id.ToString());

			var activeCharacter = discordUser.Characters.Find(c => c.IsAlive());

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

		[Command("traits")]
		public async Task ShowTraits()
		{
			var traits = await _stewardContext.Traits.ToListAsync();

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
	}
}
