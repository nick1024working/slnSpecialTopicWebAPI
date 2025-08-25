namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task<int> CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }
}
