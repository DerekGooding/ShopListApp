using HtmlAgilityPack;
using ShopListApp.Application;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Interfaces.Parsing;
using ShopListApp.Interfaces.StoreObserver;

namespace ShopListApp.Infrastructure;

public class StorePublisher(IHtmlFetcher<HtmlNode, HtmlDocument> htmlFetcher,
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IStoreRepository storeRepository) : IStorePublisher
{
    private List<IStoreSubscriber> _subscribers = new List<IStoreSubscriber>();
    private readonly IHtmlFetcher<HtmlNode, HtmlDocument> _htmlFetcher = htmlFetcher;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IStoreRepository _storeRepository = storeRepository;

    public async Task Notify()
    {
        foreach (var subscriber in _subscribers)
        {
            await subscriber.Update();
        }
    }

    public void AddSubscribers()
    {
        var biedronkaParser = new BiedronkaParser(_htmlFetcher);
        var biedronkaSubscriber = new StoreSubscriber(biedronkaParser, _productRepository, _categoryRepository, _storeRepository);
        _subscribers.Add(biedronkaSubscriber);
    }
}
