using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BelgiumPulseDbContext _context;

    public UnitOfWork(BelgiumPulseDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}