using FindLiteAI.Core.Enums;

namespace FindLiteAI.Core.Options;

/// <summary>
/// Defines configuration options for the FindLiteAI engine.
/// </summary>
public sealed class FindLiteAIOptions
{
    /// <summary>
    /// Gets or sets the LiteDB database file path.
    /// </summary>
    public string DatabasePath { get; set; } = "findliteai.db";

    /// <summary>
    /// Gets or sets the AI embedding model profile.
    /// </summary>
    public ModelProfile ModelProfile { get; set; } = ModelProfile.Balanced;

    /// <summary>
    /// Gets or sets a value indicating whether missing models
    /// can be automatically downloaded.
    /// </summary>
    public bool AllowModelDownload { get; set; } = true;

    /// <summary>
    /// Gets or sets the explicit ONNX model path.
    /// </summary>
    public string? ModelPath { get; set; }

    /// <summary>
    /// Gets or sets the local model cache directory.
    /// </summary>
    public string? ModelCacheDirectory { get; set; }
}
