using kurssienhallinta.Models;
using Microsoft.EntityFrameworkCore;

public static class DbContextFactory
{
    public static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        return new AppDbContext(options);
    }
}