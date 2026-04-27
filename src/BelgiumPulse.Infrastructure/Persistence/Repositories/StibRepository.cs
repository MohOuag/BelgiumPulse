using Microsoft.EntityFrameworkCore;
using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Infrastructure.Persistence.Repositories;

public class StibRepository : IStibRepository
{
    private readonly BelgiumPulseDbContext _context;

    public StibRepository(BelgiumPulseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StibLine>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.StibLines
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<StibLine?> GetByLineNumberAsync(
        string lineNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.StibLines
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.LineNumber == lineNumber, cancellationToken);
    }

    public async Task<IEnumerable<StibLine>> GetDisruptedLinesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.StibLines
            .AsNoTracking()
            .Where(l => !l.IsOperational)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        StibLine line,
        CancellationToken cancellationToken = default)
    {
        await _context.StibLines.AddAsync(line, cancellationToken);
    }

    public Task UpdateAsync(
        StibLine line,
        CancellationToken cancellationToken = default)
    {
        _context.StibLines.Update(line);
        return Task.CompletedTask;
    }
}