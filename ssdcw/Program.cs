
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ssdcw.Data;
using ssdcw.Models;

namespace ssdcw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            //var host = CreateHostBuilder(args).Build();
            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    try
            //    {
            //        // get data context 
            //        var context = services.GetRequiredService<ApplicationDbContext>();
            //        var userManager = services.GetRequiredService<UserManager<User>>();
            //        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            //        // if no db / migrations exist - create
            //        context.Database.Migrate();

            //        // seed roles                    
            //        DatabaseSeed.Seed(context, roleManager, userManager);
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            //host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
