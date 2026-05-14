namespace FindLiteAI.AspNetCore.Models;

/// <summary>
/// Represents a request to add multiple semantic documents.
/// </summary>
public sealed class AddDocumentsRequest
{
    /// <summary>
    /// Gets or sets the documents to add.
    /// </summary>
    public required IReadOnlyList<AddDocumentRequest> Documents { get; set; }
}
