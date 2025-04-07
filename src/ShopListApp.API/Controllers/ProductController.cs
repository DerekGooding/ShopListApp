using Microsoft.AspNetCore.Mvc;
using ShopListApp.Interfaces.IServices;

namespace ShopListApp.API.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IProductService productService) : ControllerBase
{
    private IProductService _productService = productService;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllProducts() => Ok(await _productService.GetAllProducts());

    [HttpGet("get-by-category/{id}")]
    public async Task<IActionResult> GetProductsByCategory(int id) => Ok(await _productService.GetProductsByCategoryId(id));

    [HttpGet("get-by-store/{id}")]
    public async Task<IActionResult> GetProductsByStore(int id) => Ok(await _productService.GetProductsByStoreId(id));

    [HttpPatch("refresh")]
    public async Task<IActionResult> RefreshProducts()
    {
        await _productService.RefreshProducts();
        return Ok();
    }

    [HttpGet("get-categories")]
    public async Task<IActionResult> GetCategories() => Ok(await _productService.GetCategories());

    [HttpGet("get-product/{id}")]
    public async Task<IActionResult> GetProductById(int id) => Ok(await _productService.GetProductById(id));
}
