using Microsoft.AspNetCore.Mvc;
using ShopListApp.Interfaces.IServices;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/store")]
public class StoreController(IStoreService storeService) : ControllerBase
{
    private IStoreService _storeService = storeService;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetStores() => Ok(await _storeService.GetStores());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStore(int id) => Ok(await _storeService.GetStoreById(id));
}
