using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Steward.Context.Models;

namespace Steward.Context
{
	public class StewardContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

			var configuration = configBuilder.Build();

			var stewardConfig = new StewardConfig();
			configuration.GetSection("StewardConfig").Bind(stewardConfig);

			optionsBuilder.UseSqlServer($"Server={stewardConfig.DatabaseIp};Database={stewardConfig.DatabaseName};User Id={stewardConfig.SqlUsername};Password={stewardConfig.SqlPassword};MultipleActiveResultSets=true");
		}

		public DbSet<CharacterDeathTimer> CharacterDeathTimers { get; set; }
		public DbSet<CharacterTrait> CharacterTraits { get; set; } //linking table
		public DbSet<DiscordUser> DiscordUsers { get; set; }
		public DbSet<Graveyard> Graveyards { get; set; }
		public DbSet<House> Houses { get; set; }
		public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
		public DbSet<StaffAction> StaffActions { get; set; }
		public DbSet<StaffActionChannel> StaffActionChannels { get; set; }
		public DbSet<Trait> Traits { get; set; }
		public DbSet<UserMessageRecord> MessageRecords { get; set; }
		public DbSet<ValkFinderWeapon> ValkFinderWeapons { get; set; }
		public DbSet<Year> Year { get; set; }
		public DbSet<ValkFinderArmour> ValkFinderArmours { get; set; }
		public DbSet<ValkFinderItem> ValkFinderItems { get; set; }
		public DbSet<CharacterInventory> CharacterInventories { get; set; }
		public DbSet<Proposal> Proposals { get; set; }
		public DbSet<MarriageChannel> MarriageChannels { get; set; }



		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CharacterDeathTimer>()
				.HasOne(cdt => cdt.PlayerCharacter)
				.WithOne()
				.HasForeignKey<CharacterDeathTimer>(cdt => cdt.PlayerCharacterId);

			modelBuilder.Entity<UserMessageRecord>()
				.HasOne(ms => ms.User)
				.WithMany(d => d.MessageRecords)
				.HasForeignKey(ms => ms.UserId);

			modelBuilder.Entity<CharacterTrait>()
				.HasOne(ct => ct.PlayerCharacter)
				.WithMany(pc => pc.CharacterTraits)
				.HasForeignKey(ct => ct.PlayerCharacterId);

			modelBuilder.Entity<CharacterTrait>()
				.HasOne(pt => pt.Trait)
				.WithMany(tr => tr.PlayerTraits)
				.HasForeignKey(pt => pt.TraitId);

			modelBuilder.Entity<CharacterTrait>()
				.HasKey(ct => new {ct.PlayerCharacterId, ct.TraitId});

			modelBuilder.Entity<PlayerCharacter>()
				.HasOne(pc => pc.DiscordUser)
				.WithMany(du => du.Characters)
				.HasForeignKey(pc => pc.DiscordUserId);

			modelBuilder.Entity<PlayerCharacter>()
				.HasOne(pc => pc.House)
				.WithMany(h => h.HouseMembers)
				.HasForeignKey(pc => pc.HouseId);

			modelBuilder.Entity<PlayerCharacter>()
				.HasOne(pc => pc.DefaultMeleeWeapon)
				.WithMany()
				.HasForeignKey(pc => pc.DefaultMeleeWeaponId);

			modelBuilder.Entity<PlayerCharacter>()
				.HasOne(pc => pc.DefaultRangedWeapon)
				.WithMany()
				.HasForeignKey(pc => pc.DefaultRangedWeaponId);

			modelBuilder.Entity<PlayerCharacter>()
				.HasOne(pc => pc.EquippedArmour)
				.WithMany()
				.HasForeignKey(pc => pc.EquippedArmourId);

			modelBuilder.Entity<PlayerCharacter>()
				.HasMany(pc => pc.CharacterInventories)
				.WithOne(ci => ci.PlayerCharacter)
				.HasForeignKey(ci => ci.PlayerCharacterId);

			modelBuilder.Entity<Proposal>()
				.HasOne(p => p.Proposer)
				.WithMany()
				.HasForeignKey(p => p.ProposerId);

			modelBuilder.Entity<Proposal>()
				.HasOne(p => p.Proposed)
				.WithMany()
				.HasForeignKey(p => p.ProposedId);

			modelBuilder.Entity<CharacterInventory>()
				.HasOne(ci => ci.ValkFinderWeapon)
				.WithMany()
				.HasForeignKey(ci => ci.ValkFinderWeaponId);

			modelBuilder.Entity<CharacterInventory>()
				.HasOne(ci => ci.ValkFinderArmour)
				.WithMany()
				.HasForeignKey(ci => ci.ValkFinderArmourId);

			modelBuilder.Entity<CharacterInventory>()
				.HasOne(ci => ci.ValkFinderItem)
				.WithMany()
				.HasForeignKey(ci => ci.ValkFinderItemId);

			modelBuilder.Entity<StaffAction>()
				.HasOne(sa => sa.Submitter)
				.WithMany(du => du.CreatedStaffActions)
				.HasForeignKey(sa => sa.SubmitterId);

			modelBuilder.Entity<StaffAction>()
				.HasOne(sa => sa.AssignedTo)
				.WithMany(du => du.AssignedStaffActions)
				.HasForeignKey(sa => sa.AssignedToId);

			modelBuilder.Entity<House>()
				.HasOne(h => h.HouseOwner)
				.WithOne()
				.HasForeignKey<House>(h => h.HouseOwnerId);

			modelBuilder.Entity<DiscordUser>()
				.HasData(new DiscordUser()
				{
					DiscordId = "75968535074967552",
					CanUseAdminCommands = true
				});

			modelBuilder.Entity<PlayerCharacter>()
				.HasData(new PlayerCharacter()
				{
					CharacterId = "123",
					CharacterName = "Maurice Wentworth",
					Bio = "Gotta go fast!",
					STR = 12,
					END = 8,
					DEX = 14,
					PER = 8,
					INT = 8,
					InitialAge = 20,
					YearOfBirth = 1980,
					DiscordUserId = "75968535074967552",
					HouseId = "123"
				});

			modelBuilder.Entity<Trait>()
				.HasData(new Trait[]
				{
					new Trait()
					{
						Id = "123",
						STR = -1,
						PER = 1,
						Description =
							"Court Education - You have been educated in the writ of law, and the book of justice or whatever."
					},
					new Trait()
					{
						Id = "124",
						STR = 1,
						INT = -1,
						Description = "Military Education - You have been educated in the martial and military."
					},
					new Trait()
					{
						Id = "125",
						END = 1,
						DEX = -1,
						Description = "Religious Education - You have been educated in the history of religion and spiritualism."
					},
					new Trait()
					{
						Id = "126",
						INT = 1,
						DEX = -1,
						Description = "Administrative Education - You have been educated in the managerial, and the clerical."
					},
					new Trait()
					{
						Id = "127",
						INT = -2,
						AbilityPointBonus = 1,
						Description = "None - You are a regressive luddite, doomed to be abused for your inhuman resistance to Mother Nature."
					}
				} );

			modelBuilder.Entity<CharacterTrait>()
				.HasData(new CharacterTrait()
				{
					PlayerCharacterId = "123",
					TraitId = "123"
				});

			modelBuilder.Entity<House>()
				.HasData(new House[]
				{
					new House()
					{
					HouseId = "123",
					HouseName = "Bob"
					},
					new House()
					{
						HouseId = "124",
						STR = 1,
						INT = -1,
						HouseName = "Shandi Yongshi"
					},
					new House()
					{
						HouseId = "125",
						DEX = 1,
						STR = -1,
						HouseName = "Dharmadhatu"
					},
					new House()
					{
						HouseId = "126",
						END = 1,
						DEX = -1,
						HouseName = "Golden Carp"
					},
					new House()
					{
						HouseId = "127",
						INT = 1,
						STR = -1,
						HouseName = "Dafeng"
					},
					new House()
					{
						HouseId = "128",
						PER = 1,
						STR = -1,
						HouseName = "Nishe"
					},
					new House()
					{
						HouseId = "129",
						DEX = 1,
						PER = -1,
						HouseName = "Harcaster"
					}
				});

			modelBuilder.Entity<Year>()
				.HasData(new Year()
				{
					StupidId = 1, //stupid
					CurrentYear = 372
				});
		}
	}
}