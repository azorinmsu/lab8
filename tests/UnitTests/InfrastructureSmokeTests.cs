using Phonebook.TestSupport;

namespace Phonebook.UnitTests;

public class InfrastructureSmokeTests
{
    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task Repository_CreatesDatabaseFile()
    {
        await using var database = new TempSqlite();
        await database.Repository.InitializeAsync();
        Assert.True(File.Exists(database.DatabasePath));
    }
}
