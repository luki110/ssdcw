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
using ssdcw.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ssdcw.Models;
using Microsoft.AspNetCore.Http;
using Identity.IdentityPolicy;
namespace ssdcw
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
            services.AddTransient<IPasswordValidator<User>, CustomPasswordPolicy>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SafariDatabase")));
            
            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                 .AddDefaultTokenProviders()
                 .AddDefaultUI()
                 .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<IdentityOptions>(opts => {
                opts.Password.RequiredLength = 8;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireDigit = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
           
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            SeedDB(serviceProvider);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
        /// <summary>
        /// Data seed
        /// </summary>
        /// <param name="serviceProvider"></param>
        private void SeedDB(IServiceProvider serviceProvider)
        {
            
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var rolesCount = roleManager.Roles.Count();
            if(rolesCount != 4)
            {
                var addAdmin = roleManager.CreateAsync(new IdentityRole("Admin"));
                addAdmin.Wait();
                if(addAdmin.IsCompletedSuccessfully)
                {
                   var addDev =  roleManager.CreateAsync(new IdentityRole("Developer"));
                    addDev.Wait();
                    if (addDev.IsCompletedSuccessfully)
                    {
                       var addTester = roleManager.CreateAsync(new IdentityRole("Tester"));
                        addTester.Wait();
                        if (addTester.IsCompletedSuccessfully)
                        {
                           var addClient = roleManager.CreateAsync(new IdentityRole("Client"));
                            addClient.Wait();
                            if (addClient.IsCompletedSuccessfully)
                            {
                                var usersCount = userManager.Users.Count();
                                if (usersCount == 0)
                                {
                                    var admin = new User()
                                    {
                                        FirstName = "Admin",
                                        LastName = "Adam",
                                        Email = "admin@safari.com",
                                        EmailConfirmed = true,
                                        UserName = "admin@safari.com",
                                        Rolename = "Admin"
                                    };

                                    var AddUser = userManager.CreateAsync(admin, "Complex_Authentication110#");
                                    AddUser.Wait();
                                    if (AddUser.IsCompletedSuccessfully)
                                    {
                                       var addToRole=  userManager.AddToRoleAsync(admin, "Admin");
                                        addToRole.Wait();
                                        if (addToRole.IsCompletedSuccessfully)
                                        {
                                            return;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
  
            }
          
        }
    }
}
