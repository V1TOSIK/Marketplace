namespace SharedKernel.Interfaces
{
    public interface IMediaManager
    {
        Task<IEnumerable<string>> GetAllEntityMediaUrls(Guid entityId, CancellationToken cancellationToken);
        Task<Dictionary<Guid,string>> GetAllMainMediaUrls(IEnumerable<Guid> entityIds, CancellationToken cancellationToken);
        Task<string> GetMainMediaUrl(Guid entityId, CancellationToken cancellationToken);
    }
}
