namespace ShopListApp.Interfaces.StoreObserver;

public interface IStorePublisher
{
    void AddSubscribers();
    Task Notify();
}
