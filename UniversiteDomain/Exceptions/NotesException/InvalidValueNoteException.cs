namespace UniversiteDomain.Exceptions.NotesException;

public class InvalidValueNoteException : Exception
{
    public InvalidValueNoteException() : base() { }
    public InvalidValueNoteException(string message) : base(message) { }
    public InvalidValueNoteException(string message, Exception inner) : base(message, inner) { }
}