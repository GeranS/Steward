using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
	public class ValkFinderModule : ModuleBase<SocketCommandContext>
	{

		private readonly RollService _rollService;
		private readonly StewardContext _context;
		private readonly CharacterService _characterService;

		public ValkFinderModule(StewardContext c, RollService r, CharacterService characterService)
		{
			_rollService = r;
			_context = c;
			_characterService = characterService;
		}

		[Command("add trait")]
		[Summary("Example: -1 0 0 1 0 \"Court Education - You have been educated in the writ of law, and the book of justice or whatever.\"")]
		[RequireStewardPermission]
		public async Task CreateTrait(int str, int end, int dex, int per, int intel, string description)
		{
			var newTrait = new Trait()
			{
				STR = str,
				END = end,
				DEX = dex,
				PER = per,
				INT = intel,
				Description = description
			};

			await _context.Traits.AddAsync(newTrait);
			await _context.SaveChangesAsync();
			await ReplyAsync("Trait created.");
		}

		[Command("trait")]
		public async Task AddTraitToCharacter(string traitName)
		{
			var trait = _context.Traits.FirstOrDefault(t => t.Description.StartsWith(traitName.ToLowerInvariant()));

			if (trait == null)
			{
				await ReplyAsync($"Could not find a trait with the name {trait}.");
				return;
			}

			var discordUser = _context.DiscordUsers
				.Include(du => du.Characters)
				.ThenInclude(c => c.CharacterTraits)
				.SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

			var activeCharacter = discordUser.Characters.Find(c => c.IsAlive());

			if (activeCharacter.CharacterTraits.Select(ct => ct.Trait) is List<Trait> characterTraits)
			{
				if (characterTraits.Contains(trait))
				{
					activeCharacter.CharacterTraits.RemoveAll(ct => ct.Trait == trait);
					_context.PlayerCharacters.Update(activeCharacter);
					await _context.SaveChangesAsync();
					await ReplyAsync("Trait has been removed.");
					return;
				}
			}

			var newCharacterTrait = new CharacterTrait()
			{
				Trait = trait,
				PlayerCharacter = activeCharacter
			};

			await _context.CharacterTraits.AddAsync(newCharacterTrait);
			await _context.SaveChangesAsync();
			await ReplyAsync("Trait has been added.");
		}

		[Command("traits")]
		public async Task ShowTraits()
		{
			var traits = await _context.Traits.ToListAsync();

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
				embedBuilder.AddField(trait.Description, bonusString, false);
			}

			await ReplyAsync("", false, embedBuilder.Build(), null);
		}

		[Command("roll")]
		public async Task RollStat(string stringAttribute)
		{
			if (!Enum.TryParse(stringAttribute, true, out CharacterAttribute attribute))
			{
				await ReplyAsync("Not a valid attribute.");
				return;
			}

			var discordUser = _context.DiscordUsers
				.Include(du => du.Characters)
				.ThenInclude(c => c.CharacterTraits)
				.ThenInclude(ct => ct.Trait)
				.SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

			var activeCharacter = discordUser.Characters.Find(c => c.IsAlive());

			if (activeCharacter == null)
			{
				await ReplyAsync("Could not roll because you don't have an active character.");
				return;
			}

			var rollResult = _rollService.RollPlayerStat(attribute, activeCharacter, 20);

			var embedBuilder = new EmbedBuilder
			{
				Color = Color.Purple
			};

			embedBuilder.AddField($"{activeCharacter.CharacterName}", $"{attribute}: {rollResult}")
				.WithColor(Color.Purple);

			await ReplyAsync("", false, embedBuilder.Build(), null);
		}

		[Command("bio")]
		public async Task SetBio(string bio)
		{
			var character = 
				_context.PlayerCharacters.AsEnumerable().FirstOrDefault(pc => pc.IsAlive() && pc.DiscordUserId == Context.User.Id.ToString());

			if (character == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			if (bio.Length > 1800)
			{
				await ReplyAsync("Bio is too long");
				return;
			}

			character.Bio = bio;

			_context.PlayerCharacters.Update(character);
			await _context.SaveChangesAsync();

			await ReplyAsync("Bio set.");
		}

		[Command("me")]
		public async Task Info()
		{
			var user = _context.DiscordUsers
				.Include(du => du.Characters)
				.ThenInclude(c => c.House)
				.Include(du => du.Characters)
				.ThenInclude(c => c.CharacterTraits)
				.ThenInclude(ct => ct.Trait)
				.SingleOrDefault(u => u.DiscordId == Context.User.Id.ToString());

			var activeCharacter = user.Characters.FirstOrDefault(c => c.IsAlive());

			if (activeCharacter == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			//var year = _context.Year.SingleOrDefault();

			var embedBuilder = new EmbedBuilder
			{
				Color = Color.Purple
			};

			var characterTraits = activeCharacter.CharacterTraits.Select(ct => ct.Trait).ToList();

			var traitsListString = ".";

			if (characterTraits.Count > 0)
			{
				foreach (var trait in characterTraits)
				{
					traitsListString += trait.Description + "\n";
				}
			}

			embedBuilder.AddField(_characterService.ComposeStatEmbedField(activeCharacter));

			_ = embedBuilder.AddField("Traits", traitsListString)
				.WithColor(Color.Purple);

			embedBuilder.AddField("Bio", $"{activeCharacter.Bio}")
				.WithColor(Color.Purple);

			await ReplyAsync(embed: embedBuilder.Build());
		}

		[Command("create")]
		[Summary("Creates a new character, can only be done if you don't have any living characters. Example: !create \"Olgilvie Maurice Wentworth\" \"Harcaster\" 12 14 8 8 8")]
		public async Task CreateCharacter(
			[Summary("The name if your character, use quotation marks around the name if it includes a space.")] string name, 
			[Summary("The name of the house your character is part of.")] string houseName, 
			[Summary("Strength")] int str,
			[Summary("Dexterity")] int dex,
			[Summary("Endurance")] int end,
			[Summary("Perception")] int per,
			[Summary("Intelligence")] int intel)
		{
			var discordUser = _context.DiscordUsers
				.Include(du => du.Characters)
				.SingleOrDefault(du => du.DiscordId == Context.User.Id.ToString());

			var activeCharacter = discordUser.Characters.SingleOrDefault(ac => ac.IsAlive());

			if (activeCharacter != null)
			{
				await ReplyAsync($"You still have an active character named {activeCharacter.CharacterName}. Get that one killed first.");
				return;
			}

			var house = _context.Houses.FirstOrDefault(h =>
				h.HouseName == houseName);

			if (house == null)
			{
				await ReplyAsync($"Could not find house '{houseName}'.");
				return;
			}

			var totalPoints = str + dex + end + per + intel;

			if (totalPoints != 50)
			{
				await ReplyAsync("The total amount of ability points has to be 50.");
				return;
			}

			if (str > 15 || dex > 15 || end > 15 || per > 15 || intel > 15)
			{
				await ReplyAsync("An ability score cannot be higher than 15.");
				return;
			}

			if (str < 8 || dex <8 || end < 8 || per < 8 || intel < 8)
			{
				await ReplyAsync("An ability score cannot be lower than 8.");
				return;
			}

			var year = _context.Year.First();
			var randomStartingAge = new Random().Next(18, 25);
			var randomBirthYear = year.CurrentYear - randomStartingAge;

			var newCharacter = new PlayerCharacter()
			{
				CharacterName = name,
				House = house,
				HouseId = house.HouseId,
				DiscordUser = discordUser,
				DiscordUserId = discordUser.DiscordId,
				YearOfBirth = randomBirthYear,
				STR = str,
				DEX = dex,
				END = end,
				PER = per,
				INT = intel
			};

			_context.PlayerCharacters.Add(newCharacter);
			_context.SaveChanges();

			await ReplyAsync($"Created character with the name {newCharacter.CharacterName}.");
		}
	}
}