using ContactAppWeb.Data;
using ContactAppWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace ContactAppWeb.Controllers
{
    public class ContactController : Controller
    {
        private readonly DataContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContactController(DataContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Contact/Index
        public async Task<IActionResult> Index(string sortBy, int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            var contacts = _db.ContactModels.AsQueryable();

            if (user != null)
            {
                contacts = contacts.Where(c => c.UserId == user.Id);
            }
            else
            {
                contacts = contacts.Where(c => c.UserId == null); // Show only sandbox contacts for unauthenticated users
            }

            if (sortBy == "FirstName")
            {
                contacts = contacts.OrderBy(c => c.FirstName);
            }
            else if (sortBy == "LastName")
            {
                contacts = contacts.OrderBy(c => c.LastName);
            }
            else
            {
                contacts = contacts.OrderBy(c => c.FirstName);
            }

            var pageNumber = page ?? 1;
            var pageSize = 10;
            var pagedList = contacts.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        // GET: /Contact/SearchResults
        public async Task<IActionResult> SearchResults(string searchUserInput, int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.SearchUserInput = searchUserInput;

            var contacts = _db.ContactModels.AsQueryable();

            if (user != null)
            {
                contacts = contacts.Where(c => c.UserId == user.Id);
            }
            else
            {
                contacts = contacts.Where(c => c.UserId == null); // Show only sandbox contacts for unauthenticated users
            }

            if (!string.IsNullOrEmpty(searchUserInput))
            {
                contacts = contacts.Where(c =>
                    c.FirstName.ToLower().Contains(searchUserInput.ToLower()) ||
                    c.LastName.ToLower().Contains(searchUserInput.ToLower()) ||
                    c.Email.ToLower().Contains(searchUserInput.ToLower()) ||
                    c.PhoneNumber.ToLower().Contains(searchUserInput.ToLower()))
                    .OrderBy(c => c.FirstName);
            }
            else
            {
                return View("NoResultsFound");
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;
            var pagedList = contacts.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        // GET: /Contact/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Contact/Create
        [HttpPost]
        public async Task<IActionResult> Create(ContactModel contact)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    contact.UserId = user.Id;
                }

                _db.ContactModels.Add(contact);
                await _db.SaveChangesAsync();
                TempData["success"] = "Contact created successfully.";
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: /Contact/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            ContactModel contactFromDb = null;

            if (user != null)
            {
                contactFromDb = _db.ContactModels.FirstOrDefault(c => c.Id == id && c.UserId == user.Id);
            }
            else
            {
                contactFromDb = _db.ContactModels.FirstOrDefault(c => c.Id == id && c.UserId == null);
            }

            if (contactFromDb == null)
            {
                return NotFound();
            }

            return View(contactFromDb);
        }

        // POST: /Contact/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    var originalContact = _db.ContactModels.AsNoTracking().FirstOrDefault(c => c.Id == contact.Id && c.UserId == user.Id);
                    if (originalContact != null)
                    {
                        contact.UserId = user.Id;
                    }
                }
                else
                {
                    var originalContact = _db.ContactModels.AsNoTracking().FirstOrDefault(c => c.Id == contact.Id && c.UserId == null);
                    if (originalContact == null)
                    {
                        return BadRequest("Invalid operation for unauthenticated user.");
                    }
                    contact.UserId = null;
                }

                _db.ContactModels.Update(contact);
                await _db.SaveChangesAsync();
                TempData["success"] = "Contact updated successfully.";
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: /Contact/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            ContactModel contactFromDb = null;

            if (user != null)
            {
                contactFromDb = _db.ContactModels.FirstOrDefault(c => c.Id == id && c.UserId == user.Id);
            }
            else
            {
                contactFromDb = _db.ContactModels.FirstOrDefault(c => c.Id == id && c.UserId == null);
            }

            if (contactFromDb == null)
            {
                return NotFound();
            }

            return View(contactFromDb);
        }

        // POST: /Contact/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            ContactModel contactFromDb = null;

            if (user != null)
            {
                contactFromDb = _db.ContactModels.FirstOrDefault(c => c.Id == id && c.UserId == user.Id);
            }
            else
            {
                contactFromDb = _db.ContactModels.FirstOrDefault(c => c.Id == id && c.UserId == null);
            }

            if (contactFromDb == null)
            {
                return NotFound();
            }

            _db.ContactModels.Remove(contactFromDb);
            _db.SaveChanges();
            TempData["delete"] = "Contact deleted successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Contact/ResetContactsData
        public IActionResult ResetContactsData()
        {
            DataSeeder.SeedInitialContacts(_db, HttpContext.RequestServices);
            TempData["success"] = "Contacts reset successfully.";
            return RedirectToAction("Index");
        }
    }
}
