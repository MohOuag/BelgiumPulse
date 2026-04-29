namespace BelgiumPulse.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"{name} avec l'identifiant '{key}' est introuvable.")
    {
    }
}