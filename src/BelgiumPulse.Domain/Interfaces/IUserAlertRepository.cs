using BelgiumPulse.Domain.Entities;

namespace BelgiumPulse.Domain.Interfaces;

public interface IUserAlertRepository
{
    Task<IEnumerable<UserAlert>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAlert>> GetActiveAlertsByTypeAsync(string alertType, string targetId, CancellationToken cancellationToken = default);
    Task AddAsync(UserAlert alert, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserAlert alert, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}