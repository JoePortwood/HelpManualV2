using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HelpManual.Entities;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using HelpManual.Models;
using Microsoft.AspNetCore.Server.IISIntegration;
using HelpManual.Helpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace HelpManual
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HelpManualDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HelpManualConnection")));

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // To test set a short timeout.
                options.IdleTimeout = TimeSpan.FromSeconds(10000);
                options.Cookie.HttpOnly = true;
            });

            services.AddMvc().AddFluentValidation(fv =>
            {
                //This is required due to a FluentAPI error
                fv.ConfigureClientsideValidation(enabled: false);
            });

            services.AddTransient<IValidator<ObjectTypeViewModel>, ObjectTypeViewModelValidator>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin",
                                  policy => policy.Requirements.Add(new ApplicationConfiguration(GetAdmins())));
            });
            services.AddSingleton<IAuthorizationHandler, ApplicationConfigurationHandler>();

            // IISDefaults requires the following import:
            // using Microsoft.AspNetCore.Server.IISIntegration;
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private List<string> GetAdmins()
        {
            IConfigurationSection adminSection = Configuration.GetSection("ApplicationConfiguration:AdminNames");
            IEnumerable<IConfigurationSection> adminSectionMembers = adminSection.GetChildren();
            List<string> admins = (from c in adminSectionMembers select c.Value).ToList();
            return admins;
        }
    }
}
