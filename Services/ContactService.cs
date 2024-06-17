using ContactAppWeb.Data;
using ContactAppWeb.Models;
using ContactAppWeb.Services;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

public class ContactService : IContactService
{
    private readonly DataContext _db;

    public ContactService(DataContext db)
    {
        _db = db;
    }

    public async Task<IPagedList<ContactModel>> GetContactsAsync(string sortBy, int? page, string userId)
    {
        var contacts = _db.ContactModels.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            contacts = contacts.Where(c => c.UserId == userId);
        }
        else
        {
            contacts = contacts.Where(c => c.UserId == null);
        }

        contacts = sortBy switch
        {
            "FirstName" => contacts.OrderBy(c => c.FirstName),
            "LastName" => contacts.OrderBy(c => c.LastName),
            _ => contacts.OrderBy(c => c.FirstName),
        };

        var pageNumber = page ?? 1;
        var pageSize = 10;
        return await contacts.ToPagedListAsync(pageNumber, pageSize);
    }

    public async Task<IPagedList<ContactModel>> SearchContactsAsync(string searchUserInput, int? page, string userId)
    {
        var contacts = _db.ContactModels.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            contacts = contacts.Where(c => c.UserId == userId);
        }
        else
        {
            contacts = contacts.Where(c => c.UserId == null);
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
            return await contacts.ToPagedListAsync(1, 10); // Return empty result if no search input
        }

        var pageSize = 10;
        var pageNumber = page ?? 1;
        return await contacts.ToPagedListAsync(pageNumber, pageSize);
    }

    public async Task<ContactModel> GetContactByIdAsync(int id, string userId)
    {
        return await _db.ContactModels.FirstOrDefaultAsync(c => c.Id == id && (c.UserId == userId || c.UserId == null));
    }

    public async Task<bool> CreateContactAsync(ContactModel contact, string userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            contact.UserId = userId;
        }

        _db.ContactModels.Add(contact);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateContactAsync(ContactModel contact, string userId)
    {
        var originalContact = await GetContactByIdAsync(contact.Id, userId);
        if (originalContact == null) return false;

        originalContact.FirstName = contact.FirstName;
        originalContact.LastName = contact.LastName;
        originalContact.PhoneNumber = contact.PhoneNumber;
        originalContact.Email = contact.Email;

        _db.ContactModels.Update(originalContact);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteContactAsync(int id, string userId)
    {
        var contact = await GetContactByIdAsync(id, userId);
        if (contact == null) return false;

        _db.ContactModels.Remove(contact);
        return await _db.SaveChangesAsync() > 0;
    }

    public void ResetContactsData()
    {
        DataSeeder.SeedInitialContacts(_db, null);
    }
}
