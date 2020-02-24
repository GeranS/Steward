using Microsoft.EntityFrameworkCore;
using Steward.Context.Models;

namespace Steward.Context
{
	public class StewardContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer($"Server={Properties.Resources.dbIP};Database=steward;User Id={Properties.Resources.sqlUsername};Password={Properties.Resources.sqlpassword};");
		}

		public DbSet<CharacterTrait> CharacterTraits { get; set; } //linking table
		public DbSet<DiscordUser> DiscordUsers { get; set; }
		public DbSet<Graveyard> Graveyards { get; set; }
		public DbSet<House> Houses { get; set; }
		public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
		public DbSet<StaffAction> StaffActions { get; set; }
		public DbSet<Trait> Traits { get; set; }
		public DbSet<UserMessageRecord> MessageRecords { get; set; }
		public DbSet<ValkFinderWeapon> ValkFinderWeapons { get; set; }
		public DbSet<Year> Year { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
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
				.HasData(new Trait()
				{
					Id = "123",
					STR = -1,
					PER = 1,
					Description =
						"Court Education - You have been educated in the writ of law, and the book of justice or whatever."
				});

			modelBuilder.Entity<CharacterTrait>()
				.HasData(new CharacterTrait()
				{
					PlayerCharacterId = "123",
					TraitId = "123"
				});

			modelBuilder.Entity<House>()
				.HasData(new House()
				{
					HouseId = "123",
					HouseName = "Bob"
				});

			modelBuilder.Entity<ValkFinderWeapon>()
				.HasData(new ValkFinderWeapon()
				{
					WeaponName = "Sword",
					IsRanged = false,
					DieSize = 8,
					DamageModifier = CharacterAttribute.STR
				});

			modelBuilder.Entity<Year>()
				.HasData(new Year()
				{
					CurrentYear = 2000
				});
		}
	}
}