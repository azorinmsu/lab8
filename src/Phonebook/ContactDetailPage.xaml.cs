using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Phonebook.Models;
using Phonebook.Platform;
using Phonebook.Services;
using Phonebook.Unsplash;
using Phonebook.Validation;
using System.Diagnostics;
using Contact = Phonebook.Models.Contact;

namespace Phonebook
{
    public partial class ContactDetailPage : ContentPage
    {
        private readonly Contact _contact;
        private readonly DatabaseService _databaseService;
        private readonly IUnsplashClient _unsplashClient;

        public ContactDetailPage(Contact contact)
        {
            InitializeComponent();
            _contact = contact ?? new Contact();
            _databaseService = AppServices.GetRequired<DatabaseService>();
            _unsplashClient = AppServices.GetRequired<IUnsplashClient>();
            LoadContactData();

            // Подписываемся на сообщение о выборе изображения
            MessagingCenter.Subscribe<UnsplashImagePickerPage, UnsplashImage>(
                this, "ImageSelected", async (sender, image) =>
                {
                    await HandleSelectedImage(image);
                });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Отписываемся от сообщения
            MessagingCenter.Unsubscribe<UnsplashImagePickerPage>(this, "ImageSelected");
        }

        private void LoadContactData()
        {
            nameEntry.Text = _contact.Name ?? "";
            phoneEntry.Text = _contact.PhoneNumber ?? "";
            descriptionEntry.Text = _contact.Description ?? "";
            addressEntry.Text = _contact.Address ?? "";
            emailEntry.Text = _contact.Email ?? "";

            if (!string.IsNullOrEmpty(_contact.PhotoPath))
            {
                contactImage.Source = ImageSource.FromFile(_contact.PhotoPath);
            }
            else
            {
                contactImage.Source = "default_contact.png";
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            _contact.Name = nameEntry.Text;
            _contact.PhoneNumber = phoneEntry.Text;
            _contact.Description = descriptionEntry.Text;
            _contact.Email = emailEntry.Text;
            _contact.Address = addressEntry.Text;

            await _databaseService.SaveContactAsync(_contact);
            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Удаление", "Вы уверены, что хотите удалить этот контакт?", "Да", "Нет");
            if (answer)
            {
                await _databaseService.DeleteContactAsync(_contact);
                await Navigation.PopAsync();
            }
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync();
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    contactImage.Source = ImageSource.FromStream(() => stream);
                    _contact.PhotoPath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        private async void OnCallClicked(object sender, EventArgs e)
        {
            var phoneNumber = phoneEntry.Text;
            switch (CommunicationIntents.DecideCall(PhoneDialer.Default.IsSupported, phoneNumber))
            {
                case CallAttemptResult.PhoneMissing:
                    await DisplayAlert("Ошибка", CommunicationIntents.PhoneMissingMessage, "OK");
                    return;
                case CallAttemptResult.DialerUnavailable:
                    await DisplayAlert("Ошибка", CommunicationIntents.DialerUnavailableMessage, "OK");
                    return;
            }

            if (await DisplayAlert("Позвонить", $"Позвонить на номер {phoneNumber}?", "Да", "Нет"))
            {
                try
                {
                    PhoneDialer.Default.Open(phoneNumber);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", ex.Message, "OK");
                }
            }
        }

        private void OnEmailClicked(object sender, EventArgs e)
        {
            SendEmail(emailEntry.Text?.Trim(), nameEntry.Text?.Trim());
        }

        // Вставьте этот новый метод ПРЯМО ПОД OnEmailClicked:
        private async void SendEmail(string emailAddress, string contactName = "")
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                await DisplayAlert("Информация", "Почта не указана", "OK");
                return;
            }

            if (!ContactFieldValidator.IsValidEmail(emailAddress))
            {
                await DisplayAlert("Ошибка", "Некорректный формат email", "OK");
                return;
            }

            // через Intent (только Android)
#if ANDROID
    try
    {
        var intent = new Android.Content.Intent(Android.Content.Intent.ActionSendto);
        intent.SetData(Android.Net.Uri.Parse(CommunicationIntents.BuildMailtoUri(emailAddress)));
        
        if (!string.IsNullOrWhiteSpace(contactName))
        {
            intent.PutExtra(Android.Content.Intent.ExtraSubject, $"Письмо для {contactName}");
        }
        
        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.StartActivity(intent);
    }
    catch (Android.Content.ActivityNotFoundException)
    {
        await DisplayAlert("Ошибка", CommunicationIntents.ActivityNotFoundMessage, "OK");
    }
#else
            // Для iOS и других платформ
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

        private bool IsValidEmail(string email) => ContactFieldValidator.IsValidEmail(email);

        private async Task HandleEmailException(Exception ex)
        {
            Debug.WriteLine($"Email error: {ex.Message}");

            // Проверяем тип ошибки
            if (ex.Message.Contains("No email application available", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("No installed email application", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("No application can handle this request", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("ActivityNotFoundException", StringComparison.OrdinalIgnoreCase))
            {
                await DisplayAlert("Ошибка",
                    "На устройстве не найден почтовый клиент. Установите Gmail, Outlook или другой почтовый клиент.",
                    "OK");
            }
            else if (ex.Message.Contains("user canceled", StringComparison.OrdinalIgnoreCase))
            {
                // Пользователь отменил отправку - ничего не делаем
            }
            else
            {
                await DisplayAlert("Ошибка", $"Не удалось открыть почтовый клиент: {ex.Message}", "OK");
            }
        }

        private async void OnUnsplashClicked(object sender, EventArgs e)
        {
            // Открываем страницу выбора изображений из Unsplash
            await Navigation.PushAsync(new UnsplashImagePickerPage());
        }

        private async Task HandleSelectedImage(UnsplashImage image)
        {
            try
            {
                // Показываем индикатор загрузки
                var loadingTask = DisplayAlert("Загрузка", "Скачивание изображения...", "OK");

                // Создаем уникальное имя файла
                var fileName = $"{Guid.NewGuid()}_{image.Id}.jpg";
                var localPath = await _unsplashClient.DownloadImageAsync(image.Urls.Regular, fileName);

                if (!string.IsNullOrEmpty(localPath))
                {
                    // Обновляем источник изображения на локальный файл
                    contactImage.Source = ImageSource.FromFile(localPath);
                    _contact.PhotoPath = localPath;

                    // Сохраняем информацию об изображении в описании
                    if (string.IsNullOrWhiteSpace(_contact.Description))
                    {
                        _contact.Description = $"Изображение: {image.Description ?? image.AltDescription ?? "Без описания"}";
                    }
                    else if (!_contact.Description.Contains("Изображение:"))
                    {
                        _contact.Description += $"\nИзображение: {image.Description ?? image.AltDescription ?? "Без описания"}";
                    }

                    descriptionEntry.Text = _contact.Description;

                    await DisplayAlert("Успех", "Изображение успешно загружено", "OK");
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось скачать изображение", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить изображение: {ex.Message}", "OK");
            }
        }

        // Метод для очистки изображения (если нужно)
        private void OnClearImageClicked(object sender, EventArgs e)
        {
            contactImage.Source = "default_contact.png";
            _contact.PhotoPath = null;
        }
    }
}