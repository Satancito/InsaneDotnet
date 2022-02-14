﻿using Insane.AspNet.Identity.Model1.Factory;
using Insane.EntityFrameworkCore;
using Insane.WebApiLiveTests.EntityFrameworkCore.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insane.WebApiLiveTests.EntityFrameworkCore.Factory
{
    public class IdentitySqlServerDbContextFactory : IdentityDbContextFactoryBase<IdentitySqlServerDbContext>
    {
        public override IdentitySqlServerDbContext CreateDbContext(string[] args)
        {
            var argsList = args.ToList();
            argsList.Add($"{nameof(ConfigureSettingsParameters.SecretTypes)}=\"{typeof(Program).AssemblyQualifiedName}\"");
            argsList.Add($"{nameof(ConfigureSettingsParameters.SecretTypes)}=\"{typeof(WeatherForecast).AssemblyQualifiedName}\"");
            return base.CreateDbContext(argsList.ToArray());
        }
    }
}
