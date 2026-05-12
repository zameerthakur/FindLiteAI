using FindLiteAI.Core.Exceptions;

namespace FindLiteAI.Storage.LiteDb.Internal;

/// <summary>
/// Provides validation for LiteDB semantic store options.
/// </summary>
internal static class LiteDbOptionsValidator
{
    /// <summary>
    /// Validates LiteDB semantic store options.
    /// </summary>
    /// <param name="options">
    /// The LiteDB options to validate.
    /// </param>
    /// <exception cref="SearchException">
    /// Thrown when LiteDB configuration is invalid.
    /// </exception>
    public static void Validate(
        LiteDbOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.DatabasePath))
        {
            throw new SearchException(
                "LiteDB database path must be configured.");
        }

        string? directory =
            Path.GetDirectoryName(options.DatabasePath);

        if (!string.IsNullOrWhiteSpace(directory) &&
            !Directory.Exists(directory))
        {
            throw new SearchException(
                $"LiteDB database directory does not exist: '{directory}'.");
        }

        string extension =
            Path.GetExtension(options.DatabasePath);

        if (!string.IsNullOrWhiteSpace(extension) &&
            !extension.Equals(".db", StringComparison.OrdinalIgnoreCase))
        {
            throw new SearchException(
                $"LiteDB database path must use the .db extension. Provided path: '{options.DatabasePath}'.");
        }
    }
}
