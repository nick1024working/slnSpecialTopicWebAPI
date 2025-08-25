using Microsoft.EntityFrameworkCore.Storage;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork
{
    public sealed class EfUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TeamAProjectContext _db;
        private IDbContextTransaction? _tx;

        public EfUnitOfWork(TeamAProjectContext db) => _db = db;

        /// <summary>
        /// 開啟交易，並確保不重複開啟。
        /// </summary>
        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_tx == null)
                _tx = await _db.Database.BeginTransactionAsync(ct);
        }

        /// <summary>
        /// 變更所有改變的追蹤實體，若有開啟交易則一併提交交易。
        /// </summary>
        public async Task<int> CommitAsync(CancellationToken ct = default)
        {
            var result = await _db.SaveChangesAsync(ct);

            if (_tx != null)
            {
                await _tx.CommitAsync(ct);
                await _tx.DisposeAsync();
                _tx = null;
            }

            return result;
        }

        /// <summary>
        /// 若有開啟交易則回滾交易。
        /// </summary>
        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_tx != null)
            {
                await _tx.RollbackAsync(ct);
                await _tx.DisposeAsync();
                _tx = null;
            }
        }

        public void Dispose()
        {
            _tx?.Dispose();
            _tx = null;
        }
    }

}
