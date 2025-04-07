namespace ShopListApp.Commands.Other;

public class FetchCategoryCommand
{
    public required string Name { get; set; }
    public int StoreId { get; set; }
}
