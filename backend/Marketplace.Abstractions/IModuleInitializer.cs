namespace Marketplace.Abstractions
{
    public interface IModuleInitializer
    {
        Task InitializeAsync(IServiceProvider serviceProvider);
    }
}
