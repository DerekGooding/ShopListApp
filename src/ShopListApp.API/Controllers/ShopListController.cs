﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopListApp.Commands.Create;
using ShopListApp.Commands.Update;
using ShopListApp.Interfaces.IServices;
using System.Security.Claims;

namespace ShopListApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/shoplist")]
public class ShopListController(IShopListService shopListService, IAuthorizationService authorizationService) : ControllerBase
{
    private readonly IShopListService _shopListService = shopListService;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    [HttpPost("create")]
    public async Task<IActionResult> CreateShopList([FromBody] CreateShopListCommand cmd)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _shopListService.CreateShopList(userId, cmd);
        return Ok();
    }

    [HttpDelete("delete/{shopListId}")]
    public async Task<IActionResult> DeleteShopList(int shopListId)
    {
        var shopList= await _shopListService.GetShopListById(shopListId);
        var result = await _authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await _shopListService.DeleteShopList(shopListId);
        return Ok();
    }

    [HttpPatch("update/add-product/{shopListId}/{productId}")]
    [QuantityFilter]
    public async Task<IActionResult> AddProductToShopList(int shopListId, int productId, [FromQuery] int quantity = 1)
    {
        var shopList = await _shopListService.GetShopListById(shopListId);
        var result = await _authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await _shopListService.AddProductToShopList(shopListId, productId, quantity);
        return Ok();
    }

    [HttpPatch("update/delete-product/{shopListId}/{productId}")]
    [QuantityFilter]
    public async Task<IActionResult> RemoveProductFromShopList(int shopListId, int productId, [FromQuery] int quantity = int.MaxValue)
    {
        var shopList = await _shopListService.GetShopListById(shopListId);
        var result = await _authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await _shopListService.RemoveProductFromShopList(shopListId, productId, quantity);
        return Ok();
    }

    [HttpGet("get/{shopListId}")]
    public async Task<IActionResult> GetShopList(int shopListId)
    {
        var shopList = await _shopListService.GetShopListById(shopListId);
        var result = await _authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        return !result.Succeeded ? Forbid() : Ok(shopList);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllShopLists()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var shopLists = await _shopListService.GetShopListsForUser(userId);
        return Ok(shopLists);
    }

    [HttpPut("update/{shopListId}")]
    public async Task<IActionResult> UpdateShopList(int shopListId, [FromBody] UpdateShopListCommand cmd)
    {
        var shopList = await _shopListService.GetShopListById(shopListId);
        var result = await _authorizationService.AuthorizeAsync(User, shopList, "ShopListOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        await _shopListService.UpdateShopList(shopListId, cmd);
        return Ok();
    }

}
