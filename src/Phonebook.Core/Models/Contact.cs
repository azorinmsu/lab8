using SQLite;

namespace Phonebook.Models;

/// <summary>
/// Контакт телефонной книги. Ограничение уникальности номера в схеме отсутствует.
/// </summary>
public class Contact
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string? PhotoPath { get; set; }

    [Ignore]
    public string DisplayEmail => string.IsNullOrWhiteSpace(Email) ? "Почта не указана" : Email;
}
