﻿using HtmlAgilityPack;
using ShopListApp.Interfaces.Parsing;
using System.Text;

namespace ShopListApp.Infrastructure;

public class HAPHtmlFetcher : IHtmlFetcher<HtmlNode, HtmlDocument>
{
    private HttpClient _client = null!;
    public HAPHtmlFetcher(HttpClient client)
    {
        _client = client;
    }

    public HAPHtmlFetcher() { }

    public virtual async Task<string?> FetchHtml(string baseUri, string? relativeUri = default)
    {

        var fullUri = new Uri(new Uri(baseUri), relativeUri);

        var response = await _client.GetByteArrayAsync(fullUri);
        var html = Encoding.UTF8.GetString(response);

        return html;
    }

    public string? GetAttributeValue(HtmlNode htmlNode, string attributeName)
    {
        var attributeValue = htmlNode.GetAttributeValue(attributeName, null);
        return attributeValue;
    }

    public HtmlNode? GetElementById(HtmlDocument htmlDoc, string id)
    {
        var element = htmlDoc.GetElementbyId(id);
        return element;
    }

    public ICollection<HtmlNode> GetElementsByClassName(HtmlDocument htmlDoc, string className)
    {
        var nodes = htmlDoc.DocumentNode.SelectNodes($"//*[contains(concat(' ', normalize-space(@class), ' '), ' {className} ')]");

        if (nodes == null) return new List<HtmlNode>();

        return nodes.ToList();
    }
}
