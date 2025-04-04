using System.Runtime.Serialization;

public class ForbiddenException : Exception, ISerializable
{
    public ForbiddenException() { }

    public ForbiddenException(string message) : base(message) { }

    public ForbiddenException(string message, Exception inner) : base(message, inner) { }

    protected ForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
