﻿using ShopListApp.Commands.Create;
using ShopListApp.Commands.Update;
using ShopListApp.Responses;

namespace ShopListApp.Interfaces.IServices;

public interface IShopListService
{
    Task CreateShopList(string userId, CreateShopListCommand cmd);
    Task DeleteShopList(int shopListId);
    Task AddProductToShopList(int shopListId, int productId, int quantity = 1);
    Task RemoveProductFromShopList(int shopListId, int productId, int quantity = Int32.MaxValue);
    Task<ShopListResponse> GetShopListById(int shopListId);
    Task UpdateShopList(int shopListId, UpdateShopListCommand cmd);
    Task<ICollection<ShopListResponse>> GetShopListsForUser(string userId);
}
