namespace SharedKernel.Interfaces
{
    public interface IUserManager
    {
        Task HardDeleteUser(Guid userId);
        Task SoftDeleteUser(Guid userId);
    }
}
