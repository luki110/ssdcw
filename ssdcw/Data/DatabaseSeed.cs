//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using ssdcw.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ssdcw.Data
//{
//    public class DatabaseSeed
//    {
//        public static void Seed(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
//        {

//            // if no users exist in database
//            if (!context.Users.Any())
//            {

//                IdentityRole adminRole = new IdentityRole("Admin");
//                roleManager.CreateAsync(adminRole);
//                //context.Roles.Add(adminRole);

//                IdentityRole developer = new IdentityRole("Developer");
//                roleManager.CreateAsync(developer);
//                //context.Roles.Add(developer);

//                IdentityRole tester = new IdentityRole("Tester");
//                roleManager.CreateAsync(tester);
//                //context.Roles.Add(tester);

//                IdentityRole client = new IdentityRole("Client");
//                roleManager.CreateAsync(client);
//                //context.Roles.Add(client);

                
//                var admin = new User()
//                {
//                    FirstName = "Admin",
//                    LastName = "",
//                    Email = "admin@safari.com",
//                    EmailConfirmed = true,
//                    UserName = "admin@safari.com",
//                };

//                userManager.CreateAsync(admin, "ComplexPass110#");
//                userManager.AddToRoleAsync(admin, "Admin");

//                context.SaveChanges();


//            }

//        }
//    }
//}
