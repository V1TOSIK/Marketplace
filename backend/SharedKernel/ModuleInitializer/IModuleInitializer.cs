namespace SharedKernel.ModuleInitializer
{
    public interface IModuleInitializer
    {
        Task InitializeAsync(IServiceProvider serviceProvider);
    }
}
