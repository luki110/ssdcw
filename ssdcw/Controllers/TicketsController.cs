using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ssdcw.Data;
using ssdcw.Models;
using ssdcw.Models.ViewModels;

namespace ssdcw.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public TicketsController(ApplicationDbContext context,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tickets.Include(x => x.UserAssigned).Include(x => x.Author).ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.Include(x => x.Comments).Include(x => x.Author).Include(x => x.UserAssigned)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            var comments = await _context.Comments.Include(x => x.User).Where(x => x.TicketId == id).ToListAsync();

            TicketWithCommentsVM model = new TicketWithCommentsVM();

            model.ticket = ticket;
            if (comments != null)
            {
                model.comments = comments;
            }
            model.Datecreated = ticket.DateCreated;
            model.author = ticket.Author;
            model.userAssigned = ticket.UserAssigned;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind("Content, TicketId")] Comment comment)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == comment.TicketId);
            if (ticket.TicketStatus.ToString() == "Closed")
            {
                ViewData["Error"] = "You cannot post a comment on a closed ticket";
                var ticketForModel = await _context.Tickets.Include(x => x.Comments).Include(x => x.Author).Include(x => x.UserAssigned)
                .FirstOrDefaultAsync(m => m.Id == comment.TicketId);
                if (ticket == null)
                {
                    return NotFound();
                }
                var comments = await _context.Comments.Include(x => x.User).Where(x => x.TicketId == comment.TicketId).ToListAsync();

                TicketWithCommentsVM model = new TicketWithCommentsVM();

                model.ticket = ticketForModel;
                if (comments != null)
                {
                    model.comments = comments;
                }
                model.Datecreated = ticket.DateCreated;
                model.author = ticket.Author;
                model.userAssigned = ticket.UserAssigned;

                return View(model);
            }
            var user = await GetUser();
            comment.UserId = user.Id;
            comment.DatePosted = DateTime.Now;
            comment.User = user;

            comment.Ticket = ticket;
            comment.TicketId = ticket.Id;
            ticket.Comments.ToList().Add(comment);

            if (ModelState.IsValid)
            {
                _context.Tickets.Update(ticket);
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", comment.TicketId);
            }

            return View(comment.TicketId);
        }

        [Authorize(Roles = "Admin, Tester")]
        // GET: Tickets/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            CreateTicketVM model = new CreateTicketVM();
            var users = await GetDevelopersAndTesters();

            model.users = users;
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Tester")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTicketVM model)
        {

            if (ModelState.IsValid)
            {
                var assignedUser = await _userManager.FindByNameAsync(model.userAssigned);
                var author = await GetUser();
                var ticket = new Ticket();
                ticket.Author = author;
                ticket.UserAssigned = assignedUser;
                ticket.TicketStatus = Ticket.Status.Open;
                ticket.DateCreated = DateTime.Now;
                ticket.Description = model.Description;
                if (TypeEnumExists(model.TicketType.ToString()))
                {
                    ticket.TicketType = (ssdcw.Models.Ticket.Type)model.TicketType;
                }
                else
                {
                    ViewData["Error"] = "Please select a value from dropdown list";
                    var users = await GetDevelopersAndTesters();

                    model.users = users;
                    return View(model);
                }
                if (PriorityEnumExists(model.TicketPriority.ToString()))
                {
                    ticket.TicketPriority = (ssdcw.Models.Ticket.Priority)model.TicketPriority;
                }
                else
                {
                    ViewData["Error"] = "Please select a value from dropdown list";
                    var users = await GetDevelopersAndTesters();
                    model.users = users;
                    return View(model);
                }


                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            return View(model);

        }
        [Authorize(Roles = "Admin, Tester, Developer")]
        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await GetUser();

            var ticket = await _context.Tickets.Include(x => x.Author).Include(x => x.UserAssigned).FirstOrDefaultAsync(x => x.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.Author.Id == user.Id || ticket.UserAssigned.Id == user.Id || user.Rolename == "Admin")
            {
                EditTicketVM model = new EditTicketVM();
                model.Id = ticket.Id;
                model.TicketStatus = (EditTicketVM.Status)ticket.TicketStatus;

                return View(model);

            }
            else
            {
                ViewData["Error"] = "You cannot edit this ticket";
                return RedirectToAction(nameof(Index));
            }

        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Tester, Developer")]
        public async Task<IActionResult> Edit(EditTicketVM model)
        {

            if (ModelState.IsValid)
            {
                var ticketFromDb = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == model.Id);
                ticketFromDb.TicketStatus = (Ticket.Status)model.TicketStatus;

                _context.Update(ticketFromDb);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            ViewData["Error"] = "You cannot edit this ticket";
            return View(model);
        }

        // GET: Tickets/Delete/5 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.Include(x => x.Author).Include(x => x.UserAssigned)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            var user = await GetUser();
            if (ticket.Author.Id == user.Id || ticket.UserAssigned.Id == user.Id || user.Rolename == "Admin")
            {

                return View(ticket);
            }
            else
            {
                ViewData["Error"] = "You cannot delete this ticket";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        private async Task<User> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }
        private string GetUserRole() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        private bool TypeEnumExists(string name)
        {
            if (name.Equals("Development") || name.Equals("Testing") || name.Equals("Production"))
            {
                return true;
            }
            else
                return false;
        }

        private bool PriorityEnumExists(string name)
        {
            if (name.Equals("Low") || name.Equals("Medium") || name.Equals("High"))
            {
                return true;
            }
            else
                return false;
        }
        private bool StatusEnumExists(string name)
        {
            if (name.Equals("Open") || name.Equals("In_Progress") || name.Equals("Closed"))
            {
                return true;
            }
            else
                return false;
        }
        private async Task<List<User>> GetDevelopersAndTesters()
        {
            var users = await _userManager.Users.ToListAsync();

            var devs = new List<User>();
            foreach (var user in users)
            {

                if (user.Rolename == "Developer")
                {
                    devs.Add(user);
                }
            }
            return devs;
        }
    }
}
