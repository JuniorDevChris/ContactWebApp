using ContactAppWeb.Models;
using ContactAppWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList;

namespace ContactAppWeb.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly UserManager<IdentityUser> _userManager;

        public ContactController(IContactService contactService, UserManager<IdentityUser> userManager)
        {
            _contactService = contactService;
            _userManager = userManager;
        }

        // GET: /Contact/Index
        public async Task<IActionResult> Index(string sortBy, int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            var contacts = await _contactService.GetContactsAsync(sortBy, page, userId);

            ViewData["HideSearchBar"] = !contacts.Any();
            return View(contacts);
        }

        // GET: /Contact/SearchResults
        public async Task<IActionResult> SearchResults(string searchUserInput, int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            var contacts = await _contactService.SearchContactsAsync(searchUserInput, page, userId);

            if (!contacts.Any())
            {
                return View("NoResultsFound");
            }

            ViewBag.SearchUserInput = searchUserInput;
            return View(contacts);
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
                var success = await _contactService.CreateContactAsync(contact, user?.Id);
                if (success)
                {
                    TempData["success"] = "Contact created successfully.";
                    return RedirectToAction("Index");
                }
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
            var contact = await _contactService.GetContactByIdAsync(id.Value, user?.Id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: /Contact/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                var success = await _contactService.UpdateContactAsync(contact, user?.Id);
                if (success)
                {
                    TempData["success"] = "Contact updated successfully.";
                    return RedirectToAction("Index");
                }
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
            var contact = await _contactService.GetContactByIdAsync(id.Value, user?.Id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: /Contact/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var success = await _contactService.DeleteContactAsync(id, user?.Id);
            if (success)
            {
                TempData["delete"] = "Contact deleted successfully.";
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        // GET: /Contact/ResetContactsData
        public IActionResult ResetContactsData()
        {
            _contactService.ResetContactsData();
            TempData["success"] = "Contacts reset successfully.";
            return RedirectToAction("Index");
        }
    }
}
