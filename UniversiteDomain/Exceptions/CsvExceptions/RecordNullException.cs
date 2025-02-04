namespace UniversiteDomain.Exceptions.CsvExceptions;

[Serializable]
public class RecordNullException : Exception
{
    public RecordNullException() : base() { }
    public RecordNullException(string message) : base(message) { }
    public RecordNullException(string message, Exception inner) : base(message, inner) { }
}