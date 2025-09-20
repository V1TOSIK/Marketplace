namespace SharedKernel.CurrentUser
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Role { get; }
        string? Device { get; }
        string? IpAddress { get; }
    }
}
