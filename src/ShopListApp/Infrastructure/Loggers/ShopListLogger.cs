using ShopListApp.Infrastructure.Database;
using ShopListApp.Interfaces.ILogger;
using ShopListApp.Models;

namespace ShopListApp.Infrastructure.Loggers;

public class ShopListLogger(ShopListDbContext context) : IDbLogger<ShopList>
{
    private readonly ShopListDbContext _context = context;

    public async Task Log(Operation operation, ShopList loggedObject)
    {
        var log = new ShopListLog
        {
            ShopListId = loggedObject.Id,
            Operation = operation
        };

        await _context.ShopListLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
