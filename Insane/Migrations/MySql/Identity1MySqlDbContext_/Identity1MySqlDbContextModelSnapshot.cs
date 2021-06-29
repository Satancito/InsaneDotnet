﻿// <auto-generated />
using System;
using Insane.AspNet.Identity.Model1.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Insane.Migrations.MySql.Identity1MySqlDbContext_
{
    [DbContext(typeof(Identity1MySqlDbContext))]
    partial class Identity1MySqlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Organization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset>("ActiveUntil")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("AddressLine1")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("AddresssLine2")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("varchar(128)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LogoUri")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("varchar(16)");

                    b.HasKey("Id")
                        .HasName("P_Identity.Organization_Id_57941");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Organization_Name_24069");

                    b.ToTable("Identity.Organization");

                    b
                        .HasAnnotation("Insane:AutoIncrement", 10000L);
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Permission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset>("ActiveUntil")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id")
                        .HasName("P_Identity.Permission_Id_b4baa");

                    b.HasIndex("OrganizationId")
                        .HasDatabaseName("I_Identity.Permission_OrganizationId_60c77");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("I_Identity.Permission_RoleId_ef15b");

                    b.HasIndex("UserId")
                        .HasDatabaseName("I_Identity.Permission_UserId_8dda4");

                    b.HasIndex("UserId", "RoleId", "OrganizationId")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Permission_UserId_RoleId_OrganizationId_81337");

                    b.ToTable("Identity.Permission");

                    b
                        .HasAnnotation("Insane:AutoIncrement", 10000L);
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Platform", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset>("ActiveUntil")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .IsUnicode(true)
                        .HasColumnType("varchar(512)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LogoUri")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("SecretKey")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id")
                        .HasName("P_Identity.Platform_Id_536ad");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Platform_Name_ce1d4");

                    b.HasIndex("SecretKey")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Platform_SecretKey_fee4d");

                    b.ToTable("Identity.Platform");

                    b
                        .HasAnnotation("Insane:AutoIncrement", 10000L);
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset>("ActiveUntil")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id")
                        .HasName("P_Identity.Role_Id_8e181");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Role_Name_47a7c");

                    b.ToTable("Identity.Role");

                    b
                        .HasAnnotation("Insane:AutoIncrement", 10000L);
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Session", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("ClientFriendlyName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ClientIP")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<decimal>("ClientLatitude")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("ClientLongitude")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("ClientOS")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("ClientTimezone")
                        .HasColumnType("int");

                    b.Property<string>("ClientUserAgent")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("varchar(512)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("ExpiresAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Jti")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<long>("PermissionId")
                        .HasColumnType("bigint");

                    b.Property<long>("PlatformId")
                        .HasColumnType("bigint");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<bool>("Revoked")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id")
                        .HasName("P_Identity.Session_Id_056f1");

                    b.HasIndex("Jti")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Session_Jti_991ea");

                    b.HasIndex("Key")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Session_Key_e3810");

                    b.HasIndex("PermissionId")
                        .HasDatabaseName("I_Identity.Session_PermissionId_c39fd");

                    b.HasIndex("PlatformId")
                        .HasDatabaseName("I_Identity.Session_PlatformId_9ffe5");

                    b.HasIndex("RefreshToken")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Session_RefreshToken_6c5e8");

                    b.HasIndex("TokenHash")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.Session_TokenHash_48896");

                    b.ToTable("Identity.Session");

                    b
                        .HasAnnotation("Insane:AutoIncrement", 10000L);
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("EmailConfirmationCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset>("EmailConfirmationDeadline")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset>("LockoutUntil")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("LoginFailCount")
                        .HasColumnType("int");

                    b.Property<string>("Mobile")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("varchar(16)");

                    b.Property<string>("MobileConfirmationCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset>("MobileConfirmationDeadline")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("MobileConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NormalizedEmail")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("NormalizedUsername")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Phone")
                        .HasMaxLength(16)
                        .HasColumnType("varchar(16)");

                    b.Property<string>("UniqueId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id")
                        .HasName("P_Identity.User_Id_1c7f0");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.User_Email_9daff");

                    b.HasIndex("Mobile")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.User_Mobile_75662");

                    b.HasIndex("UniqueId")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.User_UniqueId_7de64");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("U_Identity.User_Username_123f3");

                    b.ToTable("Identity.User");

                    b
                        .HasAnnotation("Insane:AutoIncrement", 10000L);
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Permission", b =>
                {
                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.Organization", "Organization")
                        .WithMany("Permissions")
                        .HasForeignKey("OrganizationId")
                        .HasConstraintName("F_Identity.Permission_OrganizationId_451b5")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.Role", "Role")
                        .WithMany("Permissions")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("F_Identity.Permission_RoleId_c6b0c")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.User", "User")
                        .WithMany("Permissions")
                        .HasForeignKey("UserId")
                        .HasConstraintName("F_Identity.Permission_UserId_e506e")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Session", b =>
                {
                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.Permission", "Permission")
                        .WithMany("Sessions")
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("F_Identity.Session_PermissionId_61c86")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.Platform", "Platform")
                        .WithMany("Sessions")
                        .HasForeignKey("PlatformId")
                        .HasConstraintName("F_Identity.Session_PlatformId_2b879")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Platform");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Organization", b =>
                {
                    b.Navigation("Permissions");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Permission", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Platform", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.Role", b =>
                {
                    b.Navigation("Permissions");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.User", b =>
                {
                    b.Navigation("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}