﻿using Insane.EntityFrameworkCore;
using Insane.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insane.AspNet.Identity.Model1.Configuration
{
    class IdentityPlatformConfiguration<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TLog> : EntityTypeConfigurationBase<TPlatform>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUserBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TLog>
        where TRole : IdentityRoleBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TLog>
        where TAccess : IdentityAccessBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TLog>
        where TUserClaim : IdentityUserClaimBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TLog>
        where TPlatform : IdentityPlatformBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TLog>
        where TSession : IdentitySessionBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TLog>
        where TLog : IdentityLogBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TLog>
    {
        public IdentityPlatformConfiguration(DatabaseFacade database, string? schema = null) : base(database, schema)
        {
        }

        public override void Configure(EntityTypeBuilder<TPlatform> builder)
        {
            builder.ToTable(Database, Schema);

            builder.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd(Database, builder, startsAt: IdentityConstants.IdentityColumnStartValue);
            builder.Ignore(e => e.UniqueId);
            builder.Property(e => e.AdminUserId).IsRequired().IsConcurrencyToken();
            builder.Property(e => e.Name).IsUnicode().IsRequired().IsConcurrencyToken();
            builder.Property(e => e.Description).IsUnicode().IsRequired(false);
            builder.Property(e => e.ApiKey).IsRequired().IsConcurrencyToken();
            builder.Property(e => e.LogoUri).IsRequired(false).IsUnicode();
            builder.Property(e => e.Type).IsRequired().IsConcurrencyToken();
            builder.Property(e => e.InDevelopment).IsRequired().IsConcurrencyToken();
            builder.Property(e => e.CanUseApiKey).IsRequired().IsConcurrencyToken();
            builder.Property(e => e.ContactEmail).IsRequired(false).IsUnicode().HasMaxLength(IdentityConstants.EmailMaxLength);
            builder.Property(e => e.Enabled).IsRequired().IsConcurrencyToken();
            builder.Property(e => e.ActiveUntil).IsRequired(false).IsConcurrencyToken();
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasPrimaryKeyIndex(Database, e => e.Id);
            builder.HasUniqueIndex(Database, e => e.Name);
            builder.HasUniqueIndex(Database, e => e.ApiKey);
            builder.HasIndex(Database, e => e.AdminUserId);
            builder.HasOne(e => e.AdminUser).WithMany(e => e.ManagedPlatforms).HasForeignKey(e => e.AdminUserId).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
            //LAST 
        }
    }
}
