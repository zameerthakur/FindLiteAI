namespace FindLiteAI.Core.Models;

/// <summary>
/// Represents a searchable document within FindLiteAI.
/// </summary>
public sealed class SemanticDocument
{
    /// <summary>
    /// Gets or sets the unique document identifier.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the searchable document content.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Gets or sets optional metadata associated with the document.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the document was created.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the UTC timestamp when the document was last updated.
    /// </summary>
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
}
