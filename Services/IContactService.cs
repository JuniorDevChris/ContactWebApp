using ContactAppWeb.Models;
using X.PagedList;

namespace ContactAppWeb.Services
{
	public interface IContactService
	{
        Task<IPagedList<ContactModel>> GetContactsAsync(string sortBy, int? page, string userId);
        Task<IPagedList<ContactModel>> SearchContactsAsync(string searchUserInput, int? page, string userId);
        Task<ContactModel> GetContactByIdAsync(int id, string userId);
        Task<bool> CreateContactAsync(ContactModel contact, string userId);
        Task<bool> UpdateContactAsync(ContactModel contact, string userId);
        Task<bool> DeleteContactAsync(int id, string userId);
        void ResetContactsData();
    }
}

