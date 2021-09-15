﻿// <auto-generated />
using System;
using Insane.AspNet.Identity.Model1.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;

namespace Insane.WebApiLiveTest.Migrations.Oracle.IdentityOracleDbContext_
{
    [DbContext(typeof(IdentityOracleDbContext))]
    partial class IdentityOracleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityOrganization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:IdentityIncrement", 1)
                        .HasAnnotation("Oracle:IdentitySeed", 10000)
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset>("ActiveUntil")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("AddressLine1")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("AddresssLine2")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("LogoUri")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR2(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("NVARCHAR2(16)");

                    b.HasKey("Id")
                        .HasName("P_IdentityOrganization_Id_DBB7E");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityOrganization_Name_F13BC");

                    b.ToTable("IdentityOrganization");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityPermission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:IdentityIncrement", 1)
                        .HasAnnotation("Oracle:IdentitySeed", 10000)
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset>("ActiveUntil")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<bool>("Enabled")
                        .HasColumnType("NUMBER(1)");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("RoleId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("UserId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id")
                        .HasName("P_IdentityPermission_Id_AA4E4");

                    b.HasIndex("OrganizationId")
                        .HasDatabaseName("I_IdentityPermission_OrganizationId_EF38A");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("I_IdentityPermission_RoleId_C1B82");

                    b.HasIndex("UserId")
                        .HasDatabaseName("I_IdentityPermission_UserId_A10AB");

                    b.HasIndex("UserId", "RoleId", "OrganizationId")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityPermission_UserId_RoleId_OrganizationId_800D6");

                    b.ToTable("IdentityPermission");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityPlatform", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:IdentityIncrement", 1)
                        .HasAnnotation("Oracle:IdentitySeed", 10000)
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset>("ActiveUntil")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(512)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("LogoUri")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR2(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("SecretKey")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.HasKey("Id")
                        .HasName("P_IdentityPlatform_Id_A8230");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityPlatform_Name_423B5");

                    b.HasIndex("SecretKey")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityPlatform_SecretKey_E10BA");

                    b.ToTable("IdentityPlatform");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:IdentityIncrement", 1)
                        .HasAnnotation("Oracle:IdentitySeed", 10000)
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset>("ActiveUntil")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<bool>("Enabled")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(128)");

                    b.HasKey("Id")
                        .HasName("P_IdentityRole_Id_5F896");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityRole_Name_36689");

                    b.ToTable("IdentityRole");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentitySession", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:IdentityIncrement", 1)
                        .HasAnnotation("Oracle:IdentitySeed", 10000)
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClientFriendlyName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("ClientIP")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("NVARCHAR2(64)");

                    b.Property<decimal>("ClientLatitude")
                        .HasColumnType("DECIMAL(18, 2)");

                    b.Property<decimal>("ClientLongitude")
                        .HasColumnType("DECIMAL(18, 2)");

                    b.Property<string>("ClientOS")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<int>("ClientTimezone")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("ClientUserAgent")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("NVARCHAR2(512)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<DateTimeOffset>("ExpiresAt")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Jti")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<long>("PermissionId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("PlatformId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<bool>("Revoked")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.HasKey("Id")
                        .HasName("P_IdentitySession_Id_ECB1D");

                    b.HasIndex("Jti")
                        .IsUnique()
                        .HasDatabaseName("U_IdentitySession_Jti_B9210");

                    b.HasIndex("Key")
                        .IsUnique()
                        .HasDatabaseName("U_IdentitySession_Key_45F50");

                    b.HasIndex("PermissionId")
                        .HasDatabaseName("I_IdentitySession_PermissionId_EB569");

                    b.HasIndex("PlatformId")
                        .HasDatabaseName("I_IdentitySession_PlatformId_0A8EA");

                    b.HasIndex("RefreshToken")
                        .IsUnique()
                        .HasDatabaseName("U_IdentitySession_RefreshToken_BC11C");

                    b.HasIndex("TokenHash")
                        .IsUnique()
                        .HasDatabaseName("U_IdentitySession_TokenHash_15499");

                    b.ToTable("IdentitySession");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:IdentityIncrement", 1)
                        .HasAnnotation("Oracle:IdentitySeed", 10000)
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("EmailConfirmationCode")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTimeOffset>("EmailConfirmationDeadline")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset>("LockoutUntil")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<int>("LoginFailCount")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("Mobile")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("NVARCHAR2(16)");

                    b.Property<string>("MobileConfirmationCode")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTimeOffset>("MobileConfirmationDeadline")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<bool>("MobileConfirmed")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("NormalizedEmail")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("NormalizedUsername")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("Password")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("Phone")
                        .HasMaxLength(16)
                        .HasColumnType("NVARCHAR2(16)");

                    b.Property<string>("UniqueId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(128)");

                    b.HasKey("Id")
                        .HasName("P_IdentityUser_Id_D06A2");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityUser_Email_A5252");

                    b.HasIndex("Mobile")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityUser_Mobile_B52FB");

                    b.HasIndex("UniqueId")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityUser_UniqueId_E083B");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("U_IdentityUser_Username_85643");

                    b.ToTable("IdentityUser");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityPermission", b =>
                {
                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.IdentityOrganization", "Organization")
                        .WithMany("Permissions")
                        .HasForeignKey("OrganizationId")
                        .HasConstraintName("F_IdentityPermission_OrganizationId_98097")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.IdentityRole", "Role")
                        .WithMany("Permissions")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("F_IdentityPermission_RoleId_63EA2")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.IdentityUser", "User")
                        .WithMany("Permissions")
                        .HasForeignKey("UserId")
                        .HasConstraintName("F_IdentityPermission_UserId_C3DFE")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentitySession", b =>
                {
                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.IdentityPermission", "Permission")
                        .WithMany("Sessions")
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("F_IdentitySession_PermissionId_68ABC")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Insane.AspNet.Identity.Model1.Entity.IdentityPlatform", "Platform")
                        .WithMany("Sessions")
                        .HasForeignKey("PlatformId")
                        .HasConstraintName("F_IdentitySession_PlatformId_83F1D")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Platform");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityOrganization", b =>
                {
                    b.Navigation("Permissions");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityPermission", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityPlatform", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityRole", b =>
                {
                    b.Navigation("Permissions");
                });

            modelBuilder.Entity("Insane.AspNet.Identity.Model1.Entity.IdentityUser", b =>
                {
                    b.Navigation("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}