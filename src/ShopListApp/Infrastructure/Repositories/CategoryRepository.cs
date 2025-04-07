using Microsoft.EntityFrameworkCore;
using ShopListApp.Infrastructure.Database;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Models;

namespace ShopListApp.Infrastructure.Repositories;

public class CategoryRepository(ShopListDbContext context) : ICategoryRepository
{
    private ShopListDbContext _context = context;

    public async Task<ICollection<Category>> GetAllCategories() => await _context.Categories.ToListAsync();

    public async Task<Category?> GetCategoryById(int? id) => await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Category?> GetCategoryByName(string? name) => await _context.Categories.FirstOrDefaultAsync(x => x.Name == name);

    public async Task AddCategory(Category category) => await _context.Categories.AddAsync(category);

    public async Task<bool> RemoveCategory(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category == null)
            return false;
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}
