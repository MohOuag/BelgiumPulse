using BelgiumPulse.Application.Common;
using BelgiumPulse.Application.Common.Exceptions;
using BelgiumPulse.Domain.Interfaces;
using MediatR;

namespace BelgiumPulse.Application.Stib.Commands.ReportDisruption;

public class ReportDisruptionHandler
    : IRequestHandler<ReportDisruptionCommand, bool>
{
    private readonly IStibRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _publisher;
    private readonly ICacheService _cache;

    public ReportDisruptionHandler(
        IStibRepository repository,
        IUnitOfWork unitOfWork,
        IMessagePublisher publisher, 
        ICacheService cache)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
        _cache = cache;
    }

    public async Task<bool> Handle(
        ReportDisruptionCommand request,
        CancellationToken cancellationToken)
    {
        var line = await _repository
            .GetByLineNumberAsync(request.LineNumber, cancellationToken);

        if (line is null)
            throw new NotFoundException(nameof(line), request.LineNumber);

        // Appel de la méthode métier sur l'entité Domain
        // qui publie aussi un DomainEvent automatiquement
        line.ReportDisruption(request.Message);

        await _repository.UpdateAsync(line, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalide le cache — les données ont changé
        await _cache.RemoveAsync(CacheKeys.AllStibLines, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.DisruptedStibLines, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.StibLineByNumber(request.LineNumber), cancellationToken);

        // Publie via l'interface
        await _publisher.PublishAsync(
            new
            {
                LineId = line.Id,
                LineNumber = line.LineNumber,
                DisruptionMessage = request.Message,
                OccurredAt = DateTime.UtcNow
            },
            routingKey: "stib.disruption.reported",
            cancellationToken: cancellationToken);

        return true;
    }
}