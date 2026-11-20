using Phonebook.Models;

namespace Phonebook.Data;

public interface IContactStore
{
    string DatabasePath { get; }

    Task InitializeAsync();

    Task<List<Contact>> GetContactsAsync();

    Task<Contact?> GetContactAsync(int id);

    Task<int> SaveContactAsync(Contact contact);

    Task<int> DeleteContactAsync(Contact contact);

    Task<List<Contact>> SearchContactsAsync(string? searchTerm);
}
