namespace Domain.Models.Exceptions;

public class BadRequestException : Exception
{
    public string Title { get; }

    public BadRequestException(string message, string title = "Bad Request") : base(message)
    {
        Title = title;
    }
}
