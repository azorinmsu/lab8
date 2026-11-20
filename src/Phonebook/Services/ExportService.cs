using ClosedXML.Excel;
using Phonebook.Models;
using System.IO;
using Contact = Phonebook.Models.Contact;

namespace Phonebook.Services
{
    public class ExportService
    {
        public async Task<string> ExportToXlsxAsync(List<Contact> contacts)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Контакты");

            // Добавляем заголовки
            worksheet.Cell(1, 1).Value = "Имя";
            worksheet.Cell(1, 2).Value = "Телефон";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Адрес";
            worksheet.Cell(1, 5).Value = "Описание";

            // Добавляем данные
            for (int i = 0; i < contacts.Count; i++)
            {
                var contact = contacts[i];
                worksheet.Cell(i + 2, 1).Value = contact.Name;
                worksheet.Cell(i + 2, 2).Value = contact.PhoneNumber;
                worksheet.Cell(i + 2, 3).Value = contact.Email;
                worksheet.Cell(i + 2, 4).Value = contact.Address;
                worksheet.Cell(i + 2, 5).Value = contact.Description;
            }

            // Форматируем заголовки
            var range = worksheet.Range(1, 1, 1, 5);
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Автоматическая ширина столбцов
            worksheet.Columns().AdjustToContents();

            // Сохраняем файл
            var fileName = $"contacts_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            workbook.SaveAs(filePath);

            return filePath;
        }
    }
} 