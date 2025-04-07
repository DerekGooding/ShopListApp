namespace ShopListApp.Interfaces.ILogger;

public interface IDbLogger<T>
{
    Task Log(Operation operation, T loggedObject);
}
