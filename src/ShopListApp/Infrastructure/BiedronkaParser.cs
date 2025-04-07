using HtmlAgilityPack;
using ShopListApp.Commands.Other;
using ShopListApp.Interfaces.Parsing;
using System.Text;

namespace ShopListApp.Infrastructure;

public class BiedronkaParser(IHtmlFetcher<HtmlNode, HtmlDocument> htmlFetcher) : IParser
{
    private readonly IHtmlFetcher<HtmlNode, HtmlDocument> _htmlFetcher = htmlFetcher;

    private IDictionary<string, string> CategoryOnPageNameToCategoryInDb()
    {
        var dict = new Dictionary<string, string>
        {
            { "warzywa", "warzywa" },
            { "owoce", "owoce" },
            { "piekarnia", "pieczywa" },
            { "nabial", "nabial i jajka" },
            { "mieso", "mieso" },
            { "dania-gotowe", "dania gotowe" },
            { "napoje", "napoje" },
            { "mrozone", "mrozone" },
            { "artykuly-spozywcze", "artykuly spozywcze" },
            { "drogeria", "drogeria" },
            { "dla-domu", "dla domu" },
            { "dla-dzieci", "dla dzieci" },
            { "dla-zwierzat", "dla zwierzat" },
        };
        return dict;
    }

    public async Task<ICollection<ParseProductCommand>> GetParsedProducts()
    {
        const string baseUri = "https://zakupy.biedronka.pl/";
        var categories = CategoryOnPageNameToCategoryInDb();
        var allProducts = new List<ParseProductCommand>();
        foreach (var category in categories)
        {
            var html = new HtmlDocument();
            var fetched = await _htmlFetcher.FetchHtml(baseUri, category.Key);
            if (fetched == null) continue;
            html.LoadHtml(fetched);
            var pages = _htmlFetcher.GetElementsByClassName(html, "bucket-pagination__link");
            var amountOfPages = int.Parse(pages.Last().InnerHtml);
            for (var i = 1; i <= amountOfPages; i++)
            {
                html = new HtmlDocument();
                var uri = $"{category.Key}?page={i}";
                fetched = await _htmlFetcher.FetchHtml(baseUri, uri);
                html.LoadHtml(fetched);
                var pageProducts = FetchProductsFromPage(html, category.Value);
                allProducts.AddRange(pageProducts);
            }
        }
        return allProducts;
    }

    private ICollection<ParseProductCommand> FetchProductsFromPage(HtmlDocument html, string? dbCategory)
    {
        var products = new List<ParseProductCommand>();
        foreach (var productHtml in _htmlFetcher.GetElementsByClassName(html, "js-product-tile"))
        {
            var productTileHtml = new HtmlDocument();
            productTileHtml.LoadHtml(productHtml.InnerHtml);
            var imageContainer = _htmlFetcher.GetElementsByClassName(productTileHtml, "tile-image__container").FirstOrDefault();
            var imgNode = imageContainer!.SelectSingleNode("//img");
            var product = new ParseProductCommand
            {
                Name = _htmlFetcher.GetElementsByClassName(productTileHtml, "product-tile__name").First().InnerHtml.Trim(),
                Price = ParsePrice(productTileHtml),
                ImageUrl = _htmlFetcher.GetAttributeValue(imgNode!, "data-srcset"),
                CategoryName = dbCategory ?? null,
                StoreId = 1
            };
            products.Add(product);
        }
        return products;
    }

    private decimal? ParsePrice(HtmlDocument htmlDoc)
    {
        var intHtml = _htmlFetcher.GetElementsByClassName(htmlDoc, "price-tile__sales").FirstOrDefault()!.InnerHtml;
        if (string.IsNullOrWhiteSpace(intHtml)) return null;
        var sb = new StringBuilder();
        foreach (var chr in intHtml)
        {
            if (chr == '<') break;
            sb.Append(chr);
        }
        ;
        var integerPart = sb.ToString().Trim();
        var decNode = _htmlFetcher.GetElementsByClassName(htmlDoc, "price-tile__decimal").FirstOrDefault();
        var decimalPart = _htmlFetcher.GetElementsByClassName(htmlDoc, "price-tile__decimal").FirstOrDefault()!.InnerHtml ?? "00";
        var fullNum = $"{integerPart},{decimalPart}";
        var result = decimal.TryParse(fullNum, out var price);
        return !result ? null : price;
    }
}
