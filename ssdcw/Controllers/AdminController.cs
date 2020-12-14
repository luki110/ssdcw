using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ssdcw.Data;
using ssdcw.Models;
using ssdcw.Models.ViewModels;

namespace ssdcw.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private IPasswordHasher<User> _passwordHasher;

        public AdminController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IPasswordHasher<User> passwordHasher,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context; 
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Index()
        {     
            return View(_userManager.Users);
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
                var user = new User
                {

                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Rolename = model.Rolename,
                    EmailConfirmed = true

                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Rolename);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
                var roles = _roleManager.Roles.ToList();
                var list = new List<string>();
                foreach (var role in roles)
                {
                    list.Add(role.Name);
                }
                model.Roles = list;
            return View(model);
        }

        public async Task<IActionResult> Edit(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            CreateUserViewModel model = new CreateUserViewModel();
            model.Email = user.Email;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password, string FirstName, string LastName)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "First Name cannot be empty");

                if (!string.IsNullOrEmpty(FirstName))
                    user.FirstName = FirstName;
                else
                    ModelState.AddModelError("", "First Name cannot be empty");

                if (!string.IsNullOrEmpty(LastName))
                    user.LastName = LastName;
                else
                    ModelState.AddModelError("", "Last Name cannot be empty");

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = _passwordHasher.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }


        //public async Task<IActionResult> DeleteUser(string Id)
        //{
        //    var user = await _userManager.FindByIdAsync(Id);
        //    return View(user);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteUser(User user)
        //{
        //    var userFromDb = await _userManager.FindByIdAsync(user.Id);

        //    //userFromDb = user;
        //    var result = await _userManager.DeleteAsync(userFromDb);

        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}



        //public async Task<IActionResult> CreateRole()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Required]string name)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
        //        if (result.Succeeded)
        //            return RedirectToAction("Index");
        //        //else
        //        //    Errors(result);
        //    }
        //    return View(name);
        //}

        public async Task<IActionResult> ChangeUserRole(string id)
        {
           
            var userToChange = await _userManager.FindByIdAsync(id);

            var roles = _roleManager.Roles.ToList();
            ChangeRoleVM model = new ChangeRoleVM();
            //var list = new List<string>();
            foreach (var role in roles)
            {
                model.Rolenames.Add(role.Name);
            }
            model.user = userToChange;
            model.previousRoleName = userToChange.Rolename;
            model.UserId = id;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(ChangeRoleVM model)
        {

            var userToChange = await _userManager.FindByIdAsync(model.UserId);
            var user = GetUser();
            if(user.Id == model.UserId)
            {
                ChangeRoleVM modelToReturn = new ChangeRoleVM(); 
                var roles = _roleManager.Roles.ToList();
                foreach (var role in roles)
                {
                    modelToReturn.Rolenames.Add(role.Name);
                }
                ViewData["Error"] = "You cannot change your own role";
                return View(modelToReturn);
            }
            if (ModelState.IsValid)
            {
                
                userToChange.Rolename = model.newRole;
                _context.Users.Update(userToChange);
                var result = await _userManager.RemoveFromRoleAsync(userToChange, model.previousRoleName);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userToChange, model.newRole);
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
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}