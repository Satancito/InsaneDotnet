﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insane.Extensions;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Insane.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text.Json.Serialization;
using System.Text.Json;
using Insane.WebApiLiveTest;

namespace Insane.WebApiTest
{
    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection services, Func<string> getConnectionStringFunction, Action<DbContextOptionsBuilder> dbContextOptionsBuilderAction = null!)
        where TContext : DbContext
        {
            Func<IServiceProvider, TContext> factory = (serviceProvider) =>
            {
                DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
                builder.UseSqlServer(getConnectionStringFunction.Invoke());
                dbContextOptionsBuilderAction.Invoke(builder);
                return (TContext)typeof(TContext).GetConstructor(new Type[] { typeof(DbContextOptions) })!.Invoke(new[] { builder.Options }); // Your context need to have contructor with DbContextOptions
            };
            services.AddScoped(factory);
            return services;
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            Configuration = new ConfigurationBuilder().
                   SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", false, true)
                   .AddUserSecrets<Program>(false, true)
                   .Build();


            DbContextSettings dbContextSettings = new DbContextSettings();
            Configuration.Bind("InsaneIdentity:DbContextSettings", dbContextSettings);
            //string migrationAssembly = typeof(Startup).Assembly.FullName;


            //DbContextFlavors<IdentityDbContextBase> flavors = DbContextFlavors<IdentityDbContextBase>
            //    .CreateInstance<IdentitySqlServerDbContext,
            //                    IdentityPostgreSqlDbContext,
            //                    IdentityMySqlDbContext,
            //                    IdentityOracleDbContext>();

            //DbContextOptionsBuilderActionFlavors builderActionFlavors = new DbContextOptionsBuilderActionFlavors()
            //{
            //    SqlServer = (options) =>
            //    {
            //        options.MigrationsAssembly(migrationAssembly);
            //    },
            //    PostgreSql = (options) =>
            //    {
            //        options.MigrationsAssembly(migrationAssembly);
            //    },
            //    MySql = (options) =>
            //    {
            //        options.MigrationsAssembly(migrationAssembly);
            //    },
            //    Oracle = (options) =>
            //    {
            //        options.MigrationsAssembly(migrationAssembly);
            //    }
            //};

            //Action<DbContextOptionsBuilder> builderAction = (options) =>
            //{
            //    options.EnableDetailedErrors();
            //    options.EnableSensitiveDataLogging();
            //};

            //services.AddDbContext(dbContextSettings, flavors, builderAction, builderActionFlavors);
            //Microsoft.AspNetCore.Identity.IdentityUser





            string getConnectionString()
            {
                DbContextSettings dbContextSettings = new DbContextSettings();
                Configuration.Bind("InsaneIdentity:DbContextSettings", dbContextSettings);
                return dbContextSettings.SqlServerConnectionString; //this is an example // Read connection string from file/config/environment
            }

            services.AddDbContext<MyContext>(getConnectionString, builder => builder.EnableDetailedErrors().EnableSensitiveDataLogging());//Dont call UseSqlServer method. It's called from AddDbContext with effective connection string




            //services.AddDbContext<IdentitySqlServerDbContext>(options =>
            //{
            //    options.UseSqlServer(dbContextSettings.SqlServerConnectionString);
            //});

            //services.AddDbContext<IdentityPostgreSqlDbContext>(options =>
            //{
            //    options.UseNpgsql(dbContextSettings.PostgreSqlConnectionString);
            //});
            //Microsoft.AspNetCore.Identity.Identity

            //services.AddIdentity<IdentityUser, IdentityRole>(op =>
            //{
            //    op.Password = new PasswordOptions
            //    {

            //    };
            //    op.Lockout = new LockoutOptions
            //    {

            //    };
            //    op.SignIn = new SignInOptions
            //    {

            //    };
            //    op.Stores = new StoreOptions
            //    {

            //    };

            //    op.Tokens = new TokenOptions
            //    {

            //    };

            //    op.ClaimsIdentity = new ClaimsIdentityOptions
            //    {
            //        EmailClaimType = System.Security.Claims.ClaimTypes.WindowsAccountName
            //    };

            //};//).AddEntityFrameworkStores<IdentityDbContext>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Insane.WebApiTest", Version = "v1" });
            });
        }

        public class Role
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MyContext context/*, IdentitySqlServerDbContext context, IdentityPostgreSqlDbContext context2*/)
        {

            context.Database.GetDbConnection().ConnectionString.WriteLine();
            //context.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Insane.WebApiTest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
