using Microsoft.EntityFrameworkCore;
using ShopListApp.Infrastructure.Database;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Models;

namespace ShopListApp.Infrastructure.Repositories;

public class StoreRepository(ShopListDbContext context) : IStoreRepository
{
    private readonly ShopListDbContext _context = context;

    public async Task<Store?> GetStoreById(int id) => await _context.Stores.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<ICollection<Store>> GetStores() => await _context.Stores.ToListAsync();
}
