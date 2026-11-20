using Phonebook.Models;

namespace Phonebook.Performance;

public static class ContactDataSetFactory
{
    public static readonly int[] StandardSizes = [10, 100, 1000, 5000, 10000];

    public static List<Contact> Create(int count, bool includePhotoPath = false)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var contacts = new List<Contact>(count);
        for (var i = 0; i < count; i++)
        {
            contacts.Add(new Contact
            {
                Name = $"Контакт {i:D5}",
                PhoneNumber = $"+7 (900) {i / 10000:000}-{i / 100 % 100:00}-{i % 100:00}",
                Email = $"user{i}@example.test",
                Address = i % 2 == 0 ? "Учебный адрес" : null,
                Description = "Сгенерированная запись для измерения",
                PhotoPath = includePhotoPath ? $"images/contact-{i:D5}.jpg" : null
            });
        }

        return contacts;
    }
}
