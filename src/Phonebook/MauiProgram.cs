using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Phonebook.Configuration;
using Phonebook.Data;
using Phonebook.Imaging;
using Phonebook.Importing;
using Phonebook.Listing;
using Phonebook.Services;
using Phonebook.Unsplash;

namespace Phonebook;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();

        var environment = PhonebookEnvironmentSelector.Current;
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<ContactImportService>();
        builder.Services.AddSingleton<ImportCoordinator>();
        builder.Services.AddSingleton<ContactListState>();
        builder.Services.AddSingleton<ExportService>();
        builder.Services.AddSingleton<MemoryImageCache>();

        if (environment == PhonebookEnvironment.Testing)
        {
            builder.Services.AddSingleton<IDeviceContactSource>(new FakeDeviceContactSource());
            builder.Services.AddSingleton<IContactPermissionGate, AllowAllContactPermissionGate>();
            builder.Services.AddSingleton<IUnsplashClient>(new FakeUnsplashClient());
        }
        else
        {
            builder.Services.AddSingleton<IDeviceContactSource, DeviceContactsService>();
            builder.Services.AddSingleton<IContactPermissionGate, MauiContactsPermissionGate>();
            builder.Services.AddSingleton<IUnsplashClient>(_ =>
            {
                var directory = Path.Combine(FileSystem.AppDataDirectory, LocalDataPlacement.ImageFolderName);
                return new UnsplashService(new HttpClient(), directory, UnsplashKeyLoader.Load());
            });
        }

#if DEBUG
        builder.Logging.AddDebug();
#endif
        var app = builder.Build();
        AppServices.Initialize(app.Services);
        return app;
    }
}
