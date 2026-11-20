using Phonebook.Models;
using Phonebook.Services;
using Phonebook.Unsplash;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Phonebook
{
    public partial class UnsplashImagePickerPage : ContentPage
    {
        private readonly IUnsplashClient _unsplashClient;
        private bool _isLoading;

        public ObservableCollection<UnsplashImage> Images { get; } = new();

        public UnsplashImagePickerPage()
        {
            try
            {
                Debug.WriteLine("UnsplashImagePickerPage constructor start");
                InitializeComponent();

                _unsplashClient = AppServices.GetRequired<IUnsplashClient>();
                BindingContext = this;

                Debug.WriteLine("Page initialized, starting to load images...");
                LoadRandomImages();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Constructor error: {ex.Message}");
                DisplayAlert("Ошибка", $"Ошибка инициализации: {ex.Message}", "OK");
            }
        }

        private async void LoadRandomImages()
        {
            Debug.WriteLine("LoadRandomImages method start");

            if (_isLoading)
            {
                Debug.WriteLine("Already loading, returning");
                return;
            }

            _isLoading = true;
            try
            {
                Debug.WriteLine("Clearing images");
                MainThread.BeginInvokeOnMainThread(() => Images.Clear());

                Debug.WriteLine("Calling GetRandomImagesAsync");
                var results = await _unsplashClient.GetRandomImagesAsync(12);
                Debug.WriteLine($"GetRandomImagesAsync returned {results?.Count ?? 0} items");

                if (results == null || results.Count == 0)
                {
                    Debug.WriteLine("No results returned");
                    await DisplayAlert("Информация", "Не удалось загрузить изображения", "OK");
                    return;
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var image in results)
                    {
                        Debug.WriteLine($"Adding image: {image.Id}, URL: {image.Urls?.Thumb}");
                        Images.Add(image);
                    }
                });

                Debug.WriteLine($"Added {results.Count} images to collection");
                Debug.WriteLine($"Current Images count: {Images.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadRandomImages: {ex.Message}");
                await DisplayAlert("Ошибка", $"Не удалось загрузить изображения: {ex.Message}", "OK");
            }
            finally
            {
                _isLoading = false;
                Debug.WriteLine("LoadRandomImages finished");
            }
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var query = e.NewTextValue?.Trim();
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                if (string.IsNullOrEmpty(query))
                {
                    LoadRandomImages();
                }
                return;
            }

            if (_isLoading) return;

            _isLoading = true;
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() => Images.Clear());

                var results = await _unsplashClient.SearchImagesAsync(query, 1, 12);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var image in results)
                    {
                        Images.Add(image);
                    }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось выполнить поиск: {ex.Message}", "OK");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnRandomClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Random button clicked");
            LoadRandomImages();
        }

        // Новый метод для обработки тапа на изображение
        private async void OnImageTapped(object sender, EventArgs e)
        {
            Debug.WriteLine("Image tapped");

            // Получаем привязанные данные
            if (sender is Frame frame && frame.BindingContext is UnsplashImage selectedImage)
            {
                await HandleImageSelection(selectedImage);
            }
            else if (sender is View view && view.BindingContext is UnsplashImage image)
            {
                await HandleImageSelection(image);
            }
        }

        private async Task HandleImageSelection(UnsplashImage selectedImage)
        {
            Debug.WriteLine($"Selected image: {selectedImage.Id}, URL: {selectedImage.Urls?.Regular}");

            // Спрашиваем подтверждение
            var confirm = await DisplayAlert("Выбор изображения",
                "Использовать это изображение для контакта?",
                "Да", "Нет");

            if (confirm)
            {
                // Возвращаем результат на предыдущую страницу
                MessagingCenter.Send(this, "ImageSelected", selectedImage);
                await Navigation.PopAsync();
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Cancel button clicked");
            await Navigation.PopAsync();
        }
    }
}