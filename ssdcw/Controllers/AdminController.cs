using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ssdcw.Data;
using ssdcw.Models;
using ssdcw.Models.ViewModels;

namespace ssdcw.Controllers
{

    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context; 
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            
            return View(users);
        }

        public async Task<IActionResult> CreateUser()
        {
            CreateUserViewModel model = new CreateUserViewModel();
            var roles = _roleManager.Roles.ToList();
            var list = new List<string>();
            foreach(var role in roles)
            {
                list.Add(role.Name);    
            }
            model.Roles = list;
           
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User {

                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,                     
                    Rolename = model.Rolename,
                    EmailConfirmed = true
                    
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Rolename);
                    return RedirectToAction(nameof(Index));
                }                
            }
                return View();
        }

        public async Task<IActionResult> Edit(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            var userFromDb = await _userManager.FindByIdAsync(user.Id);

            userFromDb = user;
            var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {                    
                    return RedirectToAction(nameof(Index));
                }
            return View();
        }
            
        

        public async Task<IActionResult> CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Required]string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                //else
                //    Errors(result);
            }
            return View(name);
        }

        public async Task<IActionResult> ChangeUserRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var roles = _roleManager.Roles.ToList();
            ChangeRoleVM model = new ChangeRoleVM();
            //var list = new List<string>();
            foreach (var role in roles)
            {
                model.Rolenames.Add(role.Name);
            }
            model.user = user;
            model.previousRoleName = user.Rolename;
            model.UserId = id;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(ChangeRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (ModelState.IsValid)
            {
                user.Rolename = model.newRole;
                _context.Users.Update(user);
                var result = await _userManager.RemoveFromRoleAsync(user, model.previousRoleName);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.newRole);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }



        private User GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            return user;
        }
        private string GetUserRole() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

    }
}