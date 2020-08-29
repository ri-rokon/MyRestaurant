using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyRestaurant.Utility;
using Stripe;
using MyRestaurant.Service;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MyRestaurant
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IDbInitializer, DbInitializer>();
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.AddSingleton< IEmailSender , EmailSender> ();
            services.Configure<EmailOptions>(Configuration);
            
            services.AddDistributedMemoryCache();
            services.AddSession(option=> {
                option.Cookie.IsEssential = true;
                option.IdleTimeout = TimeSpan.FromMinutes(30);
                option.Cookie.HttpOnly = true;
            
            });
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = "343191200203563";
                //facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = "f53426820deea684f41edb59a8581416";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["Secretkey"];
            dbInitializer.Initialize();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "Area",
                    pattern: "{area=Consumer}/{controller=Home}/{action=Index}",
                    defaults: new { Area = "Consumer", Controller = "Home", Action = "Index" }
                    );


            

                endpoints.MapControllerRoute(
                  name: "NoArea",
                  pattern: "{controller}/{action}/{ id?}",

                  defaults: new { Controller = "Home", Action = "Index" }
                  );


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");


                endpoints.MapRazorPages();
            });
        }
    }
}
