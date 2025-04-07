using ShopListApp.Commands.Other;

namespace ShopListApp.Interfaces.Parsing;

public interface IParser
{
    Task<ICollection<ParseProductCommand>> GetParsedProducts();
}
