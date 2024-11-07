namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class InvalidAnneFormationException : Exception
{
    public InvalidAnneFormationException() : base() { }
    public InvalidAnneFormationException(string message) : base(message) { }
    public InvalidAnneFormationException(string message, Exception inner) : base(message, inner) { }
}