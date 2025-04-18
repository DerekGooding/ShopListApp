﻿using Microsoft.EntityFrameworkCore;
using ShopListApp.Infrastructure.Database;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Models;

namespace ShopListApp.Infrastructure.Repositories;

public class ShopListProductRepository(ShopListDbContext context) : IShopListProductRepository
{
    private readonly ShopListDbContext _context = context;

    public async Task AddShopListProduct(ShopListProduct shopListProduct)
    {
        await _context.ShopListProducts.AddAsync(shopListProduct);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RemoveShopListProduct(int shopListId, int productId)
    {
        var shopListProduct = await _context.ShopListProducts.FirstOrDefaultAsync(x => x.ShopList.Id == shopListId
            && x.Product.Id == productId);
        if (shopListProduct == null) 
            return false;
        shopListProduct.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Product>> GetProductsForShopList(int shopListId) => await _context.ShopListProducts.Where(x => x.ShopList.Id == shopListId)
            .Include(x => x.Product)
            .ThenInclude(x => x.Store)
            .Include(x => x.Product)
            .ThenInclude(x => x.Category)
            .Select(x => x.Product).ToListAsync();

    public async Task<ShopListProduct?> GetShopListProduct(int shopListId, int productId) => await _context.ShopListProducts.FirstOrDefaultAsync(x => x.ShopList.Id == shopListId
                                                                                                  && x.Product.Id == productId);

    public async Task<bool> UpdateShopListProductQuantity(int id, int newQuantity)
    {
        var shopListProduct = await _context.ShopListProducts.FirstOrDefaultAsync(x => x.Id == id);
        if (shopListProduct == null)
            return false;
        shopListProduct.Quantity = newQuantity;
        await _context.SaveChangesAsync();
        return true;
    }

}
