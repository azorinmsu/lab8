namespace Phonebook.PerformanceTests;

public class InfrastructureSmokeTests
{
    [Fact]
    [Trait("Category", "Infrastructure")]
    public void TestProject_IsLoaded()
    {
        Assert.True(typeof(Phonebook.Data.ContactRepository).IsClass);
    }
}
