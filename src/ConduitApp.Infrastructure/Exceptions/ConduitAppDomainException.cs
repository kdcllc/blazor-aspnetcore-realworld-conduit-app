namespace ConduitApp.Infrastructure.Exceptions;

public class ConduitAppDomainException : DomainException
{
    public ConduitAppDomainException() : base()
    {
    }

    public ConduitAppDomainException(string message) : base(message)
    {
    }

    public ConduitAppDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
