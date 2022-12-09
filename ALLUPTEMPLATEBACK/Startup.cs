using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Interfaces;
using ALLUPTEMPLATEBACK.Models;
using ALLUPTEMPLATEBACK.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options=>options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }
           );

            services.AddScoped<ILayoutServices, LayoutServices>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
            });


            services.AddHttpContextAccessor();
            services.AddIdentity<AppUser, IdentityRole>(options =>
             {
                 options.Password.RequiredLength = 8;
                 options.Password.RequireLowercase = true;
                 options.Password.RequireUppercase = true;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireDigit = true;
                 options.Lockout.AllowedForNewUsers = true;
                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
                 options.Lockout.MaxFailedAccessAttempts = 5;
                 options.User.RequireUniqueEmail = true;

             }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"

            );


                endpoints.MapControllerRoute(

                    name: "default",
                    pattern: "{controller=home}/{action=index}/{id?}"
                    );

            });
        }
    }
}
