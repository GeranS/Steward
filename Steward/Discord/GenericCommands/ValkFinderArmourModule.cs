using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;

namespace Steward.Discord.GenericCommands
{
    public class ValkFinderArmourModule : ModuleBase<SocketCommandContext>
    {
        private readonly StewardContext _stewardContext;
        public ValkFinderArmourModule(StewardContext c)
        {
            _stewardContext = c;
        }

        [Command("armour equip")]
        public async Task EquipArmour(string armourName)
        {
            var activeCharacter =
                _stewardContext.PlayerCharacters
                    .Include(c => c.DefaultMeleeWeapon)
                    .Include(c => c.DefaultRangedWeapon)
                    .SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == null);

            var valkFinderArmour =
                _stewardContext.ValkFinderArmours.FirstOrDefault(w => w.ArmourName == armourName);

            if (valkFinderArmour == null)
            {
                await ReplyAsync($"Could not find Armour.");
                return;
            }

            activeCharacter.EquippedArmour = valkFinderArmour;

            _stewardContext.PlayerCharacters.Update(activeCharacter);
            await _stewardContext.SaveChangesAsync();

            await ReplyAsync($"{valkFinderArmour.ArmourName} has been equipped.");
        }

        [Command("armours")]
        public async Task ArmourList()
        {
            var embedBuilder = new EmbedBuilder();

            var valkFinderArmours = _stewardContext.ValkFinderArmours.ToList().Where(a => !a.IsUnique );

            var sortedValkFinderArmours = valkFinderArmours.OrderBy(v => v.ArmourName);

            var stringBuilder = new StringBuilder();

            foreach (var armour in sortedValkFinderArmours)
            {
                stringBuilder.AppendLine($"{armour.ArmourName}, AC: {armour.ArmourClassBonus}, DEX Penalty: {armour.DexCost}");
            }

            embedBuilder.AddField("Weapons", stringBuilder.ToString());

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("armours unique")]
        [RequireStewardPermission]
        public async Task ArmourListUnique()
        {
            var embedBuilder = new EmbedBuilder();

            var valkFinderArmours = _stewardContext.ValkFinderArmours.ToList().Where(a => a.IsUnique);

            var sortedValkFinderArmours = valkFinderArmours.OrderBy(v => v.ArmourName);

            var stringBuilder = new StringBuilder();

            foreach (var armour in sortedValkFinderArmours)
            {
                stringBuilder.AppendLine($"{armour.ArmourName}, AC: {armour.ArmourClassBonus}, DEX Penalty: {armour.DexCost}");
            }

            embedBuilder.AddField("Weapons", stringBuilder.ToString());

            await ReplyAsync(embed: embedBuilder.Build());
        }


        [Command("add armour")]
        [RequireStewardPermission]
        public async Task AddArmour(string name, int armourClass, int dexPenalty, bool unique = false)
        {
            if (name.Length > 100)
            {
                await ReplyAsync("Weapon name can't be longer than 100 characters.");
                return;
            }



            var doesArmourAlreadyExist =
                _stewardContext.ValkFinderArmours.SingleOrDefault(vfa => vfa.ArmourName == name);

            if (doesArmourAlreadyExist != null)
            {
                await ReplyAsync("Armour already exists.");
                return;
            }

            var newArmour = new ValkFinderArmour()
            {
                ArmourName = name,
                ArmourClassBonus = armourClass,
                DexCost = dexPenalty,
                IsUnique = unique
            };

            _stewardContext.ValkFinderArmours.Add(newArmour);
            await _stewardContext.SaveChangesAsync();

            await ReplyAsync("Armour added.");
        }
    }
}
