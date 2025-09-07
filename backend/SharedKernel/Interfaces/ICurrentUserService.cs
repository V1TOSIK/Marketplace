namespace SharedKernel.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Device { get; }
        string? IpAddress { get; }
    }
}
