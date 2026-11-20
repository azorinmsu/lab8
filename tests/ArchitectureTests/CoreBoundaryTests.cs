using Phonebook.Data;

namespace Phonebook.ArchitectureTests;

public class CoreBoundaryTests
{
    [Fact]
    [Trait("Category", "Architecture")]
    public void Core_DoesNotReferenceMauiOrAndroid()
    {
        var names = typeof(ContactRepository).Assembly
            .GetReferencedAssemblies()
            .Select(assembly => assembly.Name ?? string.Empty);

        Assert.DoesNotContain(names, name =>
            name.Contains("Maui", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Mono.Android", StringComparison.OrdinalIgnoreCase));
    }
}
