namespace FindLiteAI.Storage.LiteDb;

/// <summary>
/// Defines configuration options for the LiteDB semantic store.
/// </summary>
public sealed class LiteDbOptions
{
    /// <summary>
    /// Gets or sets the LiteDB database file path.
    /// </summary>
    public string DatabasePath { get; set; } = "findliteai.db";
}
