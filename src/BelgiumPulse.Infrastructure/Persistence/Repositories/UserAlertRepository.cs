using Microsoft.EntityFrameworkCore;
using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Infrastructure.Persistence.Repositories;

public class UserAlertRepository : IUserAlertRepository
{
    private readonly BelgiumPulseDbContext _context;

    public UserAlertRepository(BelgiumPulseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserAlert>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserAlerts
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserAlert>> GetActiveAlertsByTypeAsync(
        string alertType,
        string targetId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserAlerts
            .AsNoTracking()
            .Where(u => u.AlertType == alertType
                     && u.TargetId == targetId
                     && u.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        UserAlert alert,
        CancellationToken cancellationToken = default)
    {
        await _context.UserAlerts.AddAsync(alert, cancellationToken);
    }

    public Task UpdateAsync(
        UserAlert alert,
        CancellationToken cancellationToken = default)
    {
        _context.UserAlerts.Update(alert);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _context.UserAlerts
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}