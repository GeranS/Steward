﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Steward.Context;

namespace Steward.Migrations
{
    [DbContext(typeof(StewardContext))]
    partial class StewardContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Steward.Context.Models.CharacterTrait", b =>
                {
                    b.Property<string>("PlayerCharacterId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TraitId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PlayerCharacterId", "TraitId");

                    b.HasIndex("TraitId");

                    b.ToTable("CharacterTraits");

                    b.HasData(
                        new
                        {
                            PlayerCharacterId = "123",
                            TraitId = "123"
                        });
                });

            modelBuilder.Entity("Steward.Context.Models.DiscordUser", b =>
                {
                    b.Property<string>("DiscordId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("CanUseAdminCommands")
                        .HasColumnType("bit");

                    b.HasKey("DiscordId");

                    b.ToTable("DiscordUsers");

                    b.HasData(
                        new
                        {
                            DiscordId = "75968535074967552",
                            CanUseAdminCommands = true
                        });
                });

            modelBuilder.Entity("Steward.Context.Models.Graveyard", b =>
                {
                    b.Property<string>("ChannelId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ServerId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChannelId");

                    b.ToTable("Graveyards");
                });

            modelBuilder.Entity("Steward.Context.Models.House", b =>
                {
                    b.Property<string>("HouseId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AbilityPointBonus")
                        .HasColumnType("int");

                    b.Property<int>("ArmorClassBonus")
                        .HasColumnType("int");

                    b.Property<int>("DEX")
                        .HasColumnType("int");

                    b.Property<int>("END")
                        .HasColumnType("int");

                    b.Property<int>("HealthPoolBonus")
                        .HasColumnType("int");

                    b.Property<string>("HouseDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HouseName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HouseOwnerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("INT")
                        .HasColumnType("int");

                    b.Property<int>("PER")
                        .HasColumnType("int");

                    b.Property<int>("STR")
                        .HasColumnType("int");

                    b.HasKey("HouseId");

                    b.HasIndex("HouseOwnerId")
                        .IsUnique()
                        .HasFilter("[HouseOwnerId] IS NOT NULL");

                    b.ToTable("Houses");

                    b.HasData(
                        new
                        {
                            HouseId = "123",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = 0,
                            END = 0,
                            HealthPoolBonus = 0,
                            HouseDescription = "Empty.",
                            HouseName = "Bob",
                            INT = 0,
                            PER = 0,
                            STR = 0
                        },
                        new
                        {
                            HouseId = "124",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = 0,
                            END = 0,
                            HealthPoolBonus = 0,
                            HouseDescription = "Empty.",
                            HouseName = "Shandi Yongshi",
                            INT = -1,
                            PER = 0,
                            STR = 1
                        },
                        new
                        {
                            HouseId = "125",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = 1,
                            END = 0,
                            HealthPoolBonus = 0,
                            HouseDescription = "Empty.",
                            HouseName = "Dharmadhatu",
                            INT = 0,
                            PER = 0,
                            STR = -1
                        },
                        new
                        {
                            HouseId = "126",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = -1,
                            END = 1,
                            HealthPoolBonus = 0,
                            HouseDescription = "Empty.",
                            HouseName = "Golden Carp",
                            INT = 0,
                            PER = 0,
                            STR = 0
                        },
                        new
                        {
                            HouseId = "127",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = 0,
                            END = 0,
                            HealthPoolBonus = 0,
                            HouseDescription = "Empty.",
                            HouseName = "Dafeng",
                            INT = 1,
                            PER = 0,
                            STR = -1
                        },
                        new
                        {
                            HouseId = "128",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = 0,
                            END = 0,
                            HealthPoolBonus = 0,
                            HouseDescription = "Empty.",
                            HouseName = "Nishe",
                            INT = 0,
                            PER = 1,
                            STR = -1
                        },
                        new
                        {
                            HouseId = "129",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = 1,
                            END = 0,
                            HealthPoolBonus = 0,
                            HouseDescription = "Empty.",
                            HouseName = "Harcaster",
                            INT = 0,
                            PER = -1,
                            STR = 0
                        });
                });

            modelBuilder.Entity("Steward.Context.Models.PlayerCharacter", b =>
                {
                    b.Property<string>("CharacterId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CharacterName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DEX")
                        .HasColumnType("int");

                    b.Property<string>("DefaultMeleeWeaponId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DefaultRangedWeaponId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DiscordUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("END")
                        .HasColumnType("int");

                    b.Property<string>("HouseId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("INT")
                        .HasColumnType("int");

                    b.Property<int>("InitialAge")
                        .HasColumnType("int");

                    b.Property<int>("PER")
                        .HasColumnType("int");

                    b.Property<int>("STR")
                        .HasColumnType("int");

                    b.Property<int>("YearOfBirth")
                        .HasColumnType("int");

                    b.Property<string>("YearOfDeath")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CharacterId");

                    b.HasIndex("DefaultMeleeWeaponId");

                    b.HasIndex("DefaultRangedWeaponId");

                    b.HasIndex("DiscordUserId");

                    b.HasIndex("HouseId");

                    b.ToTable("PlayerCharacters");

                    b.HasData(
                        new
                        {
                            CharacterId = "123",
                            Bio = "Gotta go fast!",
                            CharacterName = "Maurice Wentworth",
                            DEX = 14,
                            DiscordUserId = "75968535074967552",
                            END = 8,
                            HouseId = "123",
                            INT = 8,
                            InitialAge = 20,
                            PER = 8,
                            STR = 12,
                            YearOfBirth = 1980
                        });
                });

            modelBuilder.Entity("Steward.Context.Models.StaffAction", b =>
                {
                    b.Property<long>("StaffActionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ActionResponse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ActionTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AssignedToId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("SubmitterId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("StaffActionId");

                    b.HasIndex("AssignedToId");

                    b.HasIndex("SubmitterId");

                    b.ToTable("StaffActions");
                });

            modelBuilder.Entity("Steward.Context.Models.Trait", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AbilityPointBonus")
                        .HasColumnType("int");

                    b.Property<int>("ArmorClassBonus")
                        .HasColumnType("int");

                    b.Property<int>("DEX")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("END")
                        .HasColumnType("int");

                    b.Property<int>("HealthPoolBonus")
                        .HasColumnType("int");

                    b.Property<int>("INT")
                        .HasColumnType("int");

                    b.Property<int>("PER")
                        .HasColumnType("int");

                    b.Property<int>("STR")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Traits");

                    b.HasData(
                        new
                        {
                            Id = "123",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = 0,
                            Description = "Court Education - You have been educated in the writ of law, and the book of justice or whatever.",
                            END = 0,
                            HealthPoolBonus = 0,
                            INT = 0,
                            PER = 1,
                            STR = -1
                        },
                        new
                        {
                            Id = "124",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = 0,
                            Description = "Military Education - You have been educated in the martial and military.",
                            END = 0,
                            HealthPoolBonus = 0,
                            INT = -1,
                            PER = 0,
                            STR = 1
                        },
                        new
                        {
                            Id = "125",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = -1,
                            Description = "Religious Education - You have been educated in the history of religion and spiritualism.",
                            END = 1,
                            HealthPoolBonus = 0,
                            INT = 0,
                            PER = 0,
                            STR = 0
                        },
                        new
                        {
                            Id = "126",
                            AbilityPointBonus = 0,
                            ArmorClassBonus = 0,
                            DEX = -1,
                            Description = "Administrative Education - You have been educated in the managerial, and the clerical.",
                            END = 0,
                            HealthPoolBonus = 0,
                            INT = 1,
                            PER = 0,
                            STR = 0
                        },
                        new
                        {
                            Id = "127",
                            AbilityPointBonus = 1,
                            ArmorClassBonus = 0,
                            DEX = 0,
                            Description = "None - You are a regressive luddite, doomed to be abused for your inhuman resistance to Mother Nature.",
                            END = 0,
                            HealthPoolBonus = 0,
                            INT = -2,
                            PER = 0,
                            STR = 0
                        });
                });

            modelBuilder.Entity("Steward.Context.Models.UserMessageRecord", b =>
                {
                    b.Property<long>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<long>("ServerId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RecordId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageRecords");
                });

            modelBuilder.Entity("Steward.Context.Models.ValkFinderWeapon", b =>
                {
                    b.Property<string>("WeaponName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DamageModifier")
                        .HasColumnType("int");

                    b.Property<int>("DieSize")
                        .HasColumnType("int");

                    b.Property<bool>("IsRanged")
                        .HasColumnType("bit");

                    b.HasKey("WeaponName");

                    b.ToTable("ValkFinderWeapons");

                    b.HasData(
                        new
                        {
                            WeaponName = "Sword",
                            DamageModifier = 0,
                            DieSize = 8,
                            IsRanged = false
                        },
                        new
                        {
                            WeaponName = "Dagger",
                            DamageModifier = 0,
                            DieSize = 6,
                            IsRanged = false
                        },
                        new
                        {
                            WeaponName = "Shortbow",
                            DamageModifier = 0,
                            DieSize = 8,
                            IsRanged = true
                        });
                });

            modelBuilder.Entity("Steward.Context.Models.Year", b =>
                {
                    b.Property<int>("StupidId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrentYear")
                        .HasColumnType("int");

                    b.HasKey("StupidId");

                    b.ToTable("Year");

                    b.HasData(
                        new
                        {
                            StupidId = 1,
                            CurrentYear = 372
                        });
                });

            modelBuilder.Entity("Steward.Context.Models.CharacterTrait", b =>
                {
                    b.HasOne("Steward.Context.Models.PlayerCharacter", "PlayerCharacter")
                        .WithMany("CharacterTraits")
                        .HasForeignKey("PlayerCharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Steward.Context.Models.Trait", "Trait")
                        .WithMany("PlayerTraits")
                        .HasForeignKey("TraitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Steward.Context.Models.House", b =>
                {
                    b.HasOne("Steward.Context.Models.PlayerCharacter", "HouseOwner")
                        .WithOne()
                        .HasForeignKey("Steward.Context.Models.House", "HouseOwnerId");
                });

            modelBuilder.Entity("Steward.Context.Models.PlayerCharacter", b =>
                {
                    b.HasOne("Steward.Context.Models.ValkFinderWeapon", "DefaultMeleeWeapon")
                        .WithMany()
                        .HasForeignKey("DefaultMeleeWeaponId");

                    b.HasOne("Steward.Context.Models.ValkFinderWeapon", "DefaultRangedWeapon")
                        .WithMany()
                        .HasForeignKey("DefaultRangedWeaponId");

                    b.HasOne("Steward.Context.Models.DiscordUser", "DiscordUser")
                        .WithMany("Characters")
                        .HasForeignKey("DiscordUserId");

                    b.HasOne("Steward.Context.Models.House", "House")
                        .WithMany("HouseMembers")
                        .HasForeignKey("HouseId");
                });

            modelBuilder.Entity("Steward.Context.Models.StaffAction", b =>
                {
                    b.HasOne("Steward.Context.Models.DiscordUser", "AssignedTo")
                        .WithMany("AssignedStaffActions")
                        .HasForeignKey("AssignedToId");

                    b.HasOne("Steward.Context.Models.DiscordUser", "Submitter")
                        .WithMany("CreatedStaffActions")
                        .HasForeignKey("SubmitterId");
                });

            modelBuilder.Entity("Steward.Context.Models.UserMessageRecord", b =>
                {
                    b.HasOne("Steward.Context.Models.DiscordUser", "User")
                        .WithMany("MessageRecords")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}