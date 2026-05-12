using MediatR;
using BelgiumPulse.Application.Common.Exceptions;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Application.Stib.Commands.ReportDisruption;

public class ReportDisruptionHandler
    : IRequestHandler<ReportDisruptionCommand, bool>
{
    private readonly IStibRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _publisher;

    public ReportDisruptionHandler(
        IStibRepository repository,
        IUnitOfWork unitOfWork,
        IMessagePublisher publisher)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
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

        // Publie via l'interface — pas de dépendance sur Infrastructure
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