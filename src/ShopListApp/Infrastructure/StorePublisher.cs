using HtmlAgilityPack;
using ShopListApp.Application;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Interfaces.Parsing;
using ShopListApp.Interfaces.StoreObserver;

namespace ShopListApp.Infrastructure;

public class StorePublisher : IStorePublisher
{
    private List<IStoreSubscriber> _subscribers = new List<IStoreSubscriber>();
    private readonly IHtmlFetcher<HtmlNode, HtmlDocument> _htmlFetcher;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStoreRepository _storeRepository;
    public StorePublisher(IHtmlFetcher<HtmlNode, HtmlDocument> htmlFetcher, 
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IStoreRepository storeRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _storeRepository = storeRepository;
        _htmlFetcher = htmlFetcher;
    }
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
