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
		private readonly StewardContext _stewardContext;
		private readonly CharacterService _characterService;

		public ValkFinderModule(StewardContext c, RollService r, CharacterService characterService)
		{
			_rollService = r;
			_stewardContext = c;
			_characterService = characterService;
		}
		
		[Command("roll")]
		public async Task RollStat(string stringAttribute)
		{
			if (!Enum.TryParse(stringAttribute, true, out CharacterAttribute attribute))
			{
				await ReplyAsync("Not a valid attribute.");
				return;
			}

			var discordUser = _stewardContext.DiscordUsers
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
			var activeCharacter =
				_stewardContext.PlayerCharacters
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

			if (activeCharacter == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			if (bio.Length > 1800)
			{
				await ReplyAsync("Bio is too long");
				return;
			}

			activeCharacter.Bio = bio;

			_stewardContext.PlayerCharacters.Update(activeCharacter);
			await _stewardContext.SaveChangesAsync();

			await ReplyAsync("Bio set.");
		}

		[Command("me")]
		public async Task Info()
		{
			var user = _stewardContext.DiscordUsers
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
			var discordUser = _stewardContext.DiscordUsers
				.Include(du => du.Characters)
				.SingleOrDefault(du => du.DiscordId == Context.User.Id.ToString());

			var activeCharacter = discordUser.Characters.SingleOrDefault(ac => ac.IsAlive());

			if (activeCharacter != null)
			{
				await ReplyAsync($"You still have an active character named {activeCharacter.CharacterName}. Get that one killed first.");
				return;
			}

			var house = _stewardContext.Houses.FirstOrDefault(h =>
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

			var year = _stewardContext.Year.First();
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

			_stewardContext.PlayerCharacters.Add(newCharacter);
			_stewardContext.SaveChanges();

			await ReplyAsync($"Created character with the name {newCharacter.CharacterName}.");
		}

		[Command("history")]
		public async Task ShowCharacterHistory()
		{
			var characters =
				_stewardContext.PlayerCharacters
					.Include(c => c.House)
					.Where(c => c.DiscordUserId == Context.User.Id.ToString()).ToList();

			var activeCharacter = characters.FirstOrDefault(cs => cs.IsAlive());

			var embedBuilder = new EmbedBuilder();

			var year = _stewardContext.Year.SingleOrDefault();

			if (activeCharacter != null)
			{
				characters.Remove(activeCharacter);

				embedBuilder.AddField($"{activeCharacter.GetAge(year.CurrentYear)}",
					$"{activeCharacter.CharacterName} of House {activeCharacter.House.HouseName}");
			}

			var sortedCharacters = characters.OrderBy(c => c.YearOfBirth);

			foreach (var character in sortedCharacters)
			{
				embedBuilder.AddField($"{character.YearOfBirth} - {character.YearOfDeath}",
					$"{character.CharacterName} of House {activeCharacter.House.HouseName}");
			}

			await ReplyAsync(embed: embedBuilder.Build());
		}
	}
}