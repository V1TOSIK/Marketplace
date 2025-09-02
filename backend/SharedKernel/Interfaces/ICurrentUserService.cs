namespace ProductModule.SharedKernel.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
