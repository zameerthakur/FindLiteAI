namespace FindLiteAI.AspNetCore.Models;

/// <summary>
/// Represents a request to add a semantic document.
/// </summary>
public sealed class AddDocumentRequest
{
    /// <summary>
    /// Gets or sets the optional document identifier.
    /// If not provided, FindLiteAI generates one automatically.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the searchable document text.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Gets or sets optional metadata values.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
