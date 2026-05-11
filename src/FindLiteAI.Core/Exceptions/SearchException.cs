namespace FindLiteAI.Core.Exceptions;

/// <summary>
/// Represents the base exception for FindLiteAI search operations.
/// </summary>
public class SearchException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchException"/> class.
    /// </summary>
    public SearchException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">
    /// The exception message.
    /// </param>
    public SearchException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchException"/> class
    /// with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">
    /// The exception message.
    /// </param>
    /// <param name="innerException">
    /// The inner exception.
    /// </param>
    public SearchException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}
