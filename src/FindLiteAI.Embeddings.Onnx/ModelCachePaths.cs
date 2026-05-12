namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Provides default model cache path helpers for FindLiteAI.
/// </summary>
public static class ModelCachePaths
{
    /// <summary>
    /// Gets the default per-user model cache directory.
    /// </summary>
    /// <returns>
    /// The default per-user model cache directory.
    /// </returns>
    public static string GetUserCacheDirectory()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FindLiteAI",
            "Models");
    }

    /// <summary>
    /// Gets the model-specific user cache directory.
    /// </summary>
    /// <param name="modelId">
    /// The model identifier.
    /// </param>
    /// <returns>
    /// The model-specific user cache directory.
    /// </returns>
    public static string GetUserModelDirectory(
        string modelId)
    {
        if (string.IsNullOrWhiteSpace(modelId))
        {
            throw new ArgumentException(
                "Model identifier cannot be null or empty.",
                nameof(modelId));
        }

        return Path.Combine(
            GetUserCacheDirectory(),
            modelId);
    }
}
