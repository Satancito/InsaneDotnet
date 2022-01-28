﻿using Insane.AspNet.Identity.Model1.Entity;
using Insane.EntityFrameworkCore;
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
    public class IdentityUserClaimConfiguration : IdentityUserClaimConfiguration<long>
    {
        public IdentityUserClaimConfiguration(DatabaseFacade database) : base(database)
        {
        }
    }

    public class IdentityUserClaimConfiguration<TKey> : IdentityUserClaimConfigurationBase<TKey, IdentityUser<TKey>, IdentityRole<TKey>, IdentityAccess<TKey>, IdentityUserClaim<TKey>, IdentityPlatform<TKey>, IdentitySession<TKey>, IdentityUserRecoveryCode<TKey>, IdentityLog<TKey>>
        where TKey : IEquatable<TKey>
    {
        public IdentityUserClaimConfiguration(DatabaseFacade database) : base(database)
        {
        }
    }

    public abstract class IdentityUserClaimConfigurationBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog> : EntityTypeConfigurationBase<TUserClaim>
         where TKey : IEquatable<TKey>
        where TUser : IdentityUserBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog>
        where TRole : IdentityRoleBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog>
        where TAccess : IdentityAccessBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog>
        where TUserClaim : IdentityUserClaimBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog>
        where TPlatform : IdentityPlatformBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog>
        where TSession : IdentitySessionBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog>
        where TRecoveryCode : IdentityUserRecoveryCodeBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog>
        where TLog : IdentityLogBase<TKey, TUser, TRole, TAccess, TUserClaim, TPlatform, TSession, TRecoveryCode, TLog>
    {
        public IdentityUserClaimConfigurationBase(DatabaseFacade database) : base(database)
        {
        }

        public override void Configure(EntityTypeBuilder<TUserClaim> builder)
        {
            builder.ToTable(Database, builder.GetSchema(Database));

            builder.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd(Database, builder, startsAt: Constants.IdentityColumnStartValue);
            builder.Ignore(e => e.UniqueId);
            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.Type).IsRequired().IsUnicode().HasMaxLength(Constants.IdentifierMaxLength);
            builder.Property(e => e.Value).IsRequired().IsUnicode().HasMaxLength(Constants.SummaryMaxLength);
            builder.Property(e => e.Enabled).IsRequired();
            builder.Property(e => e.ActiveUntil).IsRequired();
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasPrimaryKeyIndex(Database, e => e.Id);
            builder.HasUniqueIndex(Database, e => new { e.UserId, e.Type, e.Value });
            builder.HasIndex(Database, e => e.UserId);

            builder.HasOne(e => e.User).WithMany(e => e.Claims).HasForeignKey(Database, builder, e => e.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}