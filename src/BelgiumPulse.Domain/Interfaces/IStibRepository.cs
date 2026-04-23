using BelgiumPulse.Domain.Entities;

namespace BelgiumPulse.Domain.Interfaces;

public interface IStibRepository
{
    Task<IEnumerable<StibLine>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StibLine?> GetByLineNumberAsync(string lineNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<StibLine>> GetDisruptedLinesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(StibLine line, CancellationToken cancellationToken = default);
    Task UpdateAsync(StibLine line, CancellationToken cancellationToken = default);
}