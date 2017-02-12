using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MvcApi1.Data;
using Models;

namespace MvcApi1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MvcApi1DbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();

                // https://github.com/rowanmiller/UnicornStore/blob/master/UnicornStore/src/UnicornStore/Startup.cs#L107
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<MvcApi1DbContext>();
                    context.Database.Migrate();
                    if (!context.Products.Any())
                    {
                        context.Products.AddRange(
                            new Product { Id = "ID1", Name = "Product 1", Category = "Category", Price = 5 },
                            new Product { Id = "ID2", Name = "Product 2", Category = "Category", Price = 6 },
                            new Product { Id = "ID3", Name = "Product 3", Category = "Category", Price = 7 },
                            new Product { Id = "ID4", Name = "Product 4", Category = "Category", Price = 8 },
                            new Product { Id = "ID5", Name = "Product 5", Category = "Category", Price = 9 }
                        );
                        context.SaveChanges();
                    }
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000",
                RequireHttpsMetadata = false,
                ApiName = "mvcapi1",
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
