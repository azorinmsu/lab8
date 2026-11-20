namespace Phonebook.Services;

public static class AppServices
{
    public static IServiceProvider Services { get; private set; } = Empty.Instance;

    public static void Initialize(IServiceProvider services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public static T GetRequired<T>() where T : notnull => Services.GetRequiredService<T>();

    private sealed class Empty : IServiceProvider
    {
        public static readonly Empty Instance = new();

        public object? GetService(Type serviceType) => null;
    }
}
