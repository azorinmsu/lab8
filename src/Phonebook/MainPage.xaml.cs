using Microsoft.Maui.ApplicationModel.Communication;
using Phonebook.Importing;
using Phonebook.Listing;
using Phonebook.Models;
using Phonebook.Platform;
using Phonebook.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Contact = Phonebook.Models.Contact;

namespace Phonebook;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly IDeviceContactSource _deviceContacts;
    private readonly IContactPermissionGate _permissionGate;
    private readonly ContactImportService _importService;
    private readonly ImportCoordinator _importCoordinator;
    private readonly ExportService _exportService;
    private readonly ContactListState _listState;
    private readonly ObservableCollection<Contact> _allContacts;
    private List<Contact> _originalContacts;

    public MainPage()
    {
        InitializeComponent();
        _databaseService = AppServices.GetRequired<DatabaseService>();
        _deviceContacts = AppServices.GetRequired<IDeviceContactSource>();
        _permissionGate = AppServices.GetRequired<IContactPermissionGate>();
        _importService = AppServices.GetRequired<ContactImportService>();
        _importCoordinator = AppServices.GetRequired<ImportCoordinator>();
        _exportService = AppServices.GetRequired<ExportService>();
        _listState = AppServices.GetRequired<ContactListState>();
        _allContacts = new ObservableCollection<Contact>();
        _originalContacts = [];
        contactsList.ItemsSource = _allContacts;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadContacts();
    }

    private async Task LoadContacts()
    {
        try
        {
            _originalContacts = await _databaseService.GetContactsAsync();
            ReplaceVisible(_listState.Filter(_originalContacts, searchBar.Text), filtered: false);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить контакты: {ex.Message}", "OK");
            Debug.WriteLine($"Ошибка загрузки контактов: {ex.Message}");
        }
    }

    private async void OnContactSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Contact selectedContact)
        {
            await Navigation.PushAsync(new ContactDetailPage(selectedContact));
            contactsList.SelectedItem = null;
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var filtered = !string.IsNullOrWhiteSpace(e.NewTextValue?.Trim());
        ReplaceVisible(_listState.Filter(_originalContacts, e.NewTextValue), filtered);
    }

    private void ReplaceVisible(IReadOnlyList<Contact> contacts, bool filtered)
    {
        _allContacts.Clear();
        foreach (var contact in contacts)
            _allContacts.Add(contact);

        contactsCountLabel.Text = _listState.FormatCount(_allContacts.Count, filtered);
    }

    private async void OnAddContactClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ContactDetailPage(new Contact()));
    }

    private async void OnImportClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await _importCoordinator.RunAsync(
                _permissionGate,
                _deviceContacts,
                _importService,
                _databaseService);

            if (result == null)
            {
                await DisplayAlert("Ошибка", "Необходим доступ к контактам", "OK");
                return;
            }

            if (result.ImportedCount == 0 && result.DuplicateCount == 0 && result.SkippedCount == 0)
            {
                await DisplayAlert("Информация", "Не найдено контактов для импорта", "OK");
                return;
            }

            await LoadContacts();
            await DisplayAlert(
                "Импорт завершен",
                $"Успешно импортировано: {result.ImportedCount} контактов\nПропущено дубликатов: {result.DuplicateCount}",
                "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка импорта: {ex}");
            await DisplayAlert("Ошибка", $"Ошибка при импорте: {ex.Message}", "OK");
        }
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        try
        {
            if (_allContacts.Count == 0)
            {
                await DisplayAlert("Информация", "Нет контактов для экспорта", "OK");
                return;
            }

            var filePath = await _exportService.ExportToXlsxAsync(_allContacts.ToList());
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Экспорт контактов",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }

    private async void OnQuickCallClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Contact contact)
            await TryCallAsync(contact.PhoneNumber);
    }

    private async Task TryCallAsync(string? phoneNumber)
    {
        var decision = CommunicationIntents.DecideCall(PhoneDialer.Default.IsSupported, phoneNumber);
        switch (decision)
        {
            case CallAttemptResult.PhoneMissing:
                await DisplayAlert("Ошибка", CommunicationIntents.PhoneMissingMessage, "OK");
                return;
            case CallAttemptResult.DialerUnavailable:
                await DisplayAlert("Ошибка", CommunicationIntents.DialerUnavailableMessage, "OK");
                return;
        }

        if (!await DisplayAlert("Позвонить", $"Позвонить на номер {phoneNumber}?", "Да", "Нет"))
            return;

        try
        {
            PhoneDialer.Default.Open(phoneNumber);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }

    private async void OnQuickEmailClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Contact contact)
            await SendEmailAsync(contact.Email, contact.Name);
    }

    private async Task SendEmailAsync(string? emailAddress, string? contactName)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            await DisplayAlert("Информация", "Почта не указана", "OK");
            return;
        }

        if (!Validation.ContactFieldValidator.IsValidEmail(emailAddress))
        {
            await DisplayAlert("Ошибка", "Некорректный формат email", "OK");
            return;
        }

#if ANDROID
        try
        {
            var intent = new Android.Content.Intent(Android.Content.Intent.ActionSendto);
            intent.SetData(Android.Net.Uri.Parse(CommunicationIntents.BuildMailtoUri(emailAddress)));
            if (!string.IsNullOrWhiteSpace(contactName))
                intent.PutExtra(Android.Content.Intent.ExtraSubject, $"Письмо для {contactName}");

            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.StartActivity(intent);
        }
        catch (Android.Content.ActivityNotFoundException)
        {
            await DisplayAlert("Ошибка", CommunicationIntents.ActivityNotFoundMessage, "OK");
        }
#else
        try
        {
            var emailMessage = new EmailMessage
            {
                Subject = string.IsNullOrWhiteSpace(contactName) ? "Письмо" : $"Письмо для {contactName}",
                Body = "",
                To = new List<string> { emailAddress }
            };
            await Email.Default.ComposeAsync(emailMessage);
        }
        catch (Exception)
        {
            await DisplayAlert("Ошибка", "Не удалось открыть почтовый клиент", "OK");
        }
#endif
    }
}
