using ShopListApp.Infrastructure;

namespace ShopListApp.TestUtilities.Stubs;

public class BiedronkaHtmlFetcherStub : HAPHtmlFetcher
{
    public override async Task<string?> FetchHtml(string url, string? uri = null)
    {
        if (url == "https://zakupy.biedronka.pl/" && uri!.Contains("warzywa"))
        {
            var filePath = "TestData/BiedronkaPage.html";
            using var reader = new StreamReader(filePath, true);
            var html = await reader.ReadToEndAsync();
            return html;
        }
        return null;
    }
}
