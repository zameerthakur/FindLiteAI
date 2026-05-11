using Microsoft.ML.OnnxRuntime;

namespace FindLiteAI.Embeddings.Onnx.Internal;

/// <summary>
/// Provides ONNX model loading and validation functionality.
/// </summary>
internal static class ModelLoader
{
    /// <summary>
    /// Loads an ONNX inference session from the specified model path.
    /// </summary>
    /// <param name="options">
    /// The ONNX embedding provider configuration.
    /// </param>
    /// <returns>
    /// An initialized ONNX inference session.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the model file does not exist.
    /// </exception>
    public static InferenceSession LoadSession(
        OnnxEmbeddingProviderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!File.Exists(options.ModelPath))
        {
            throw new FileNotFoundException(
                "The ONNX model file could not be found.",
                options.ModelPath);
        }

        SessionOptions sessionOptions = CreateSessionOptions();

        return new InferenceSession(
            options.ModelPath,
            sessionOptions);
    }

    /// <summary>
    /// Creates optimized ONNX Runtime session options.
    /// </summary>
    /// <returns>
    /// Configured ONNX Runtime session options.
    /// </returns>
    private static SessionOptions CreateSessionOptions()
    {
        SessionOptions options = new()
        {
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
        };

        options.EnableMemoryPattern = true;

        options.EnableCpuMemArena = true;

        options.ExecutionMode = ExecutionMode.ORT_SEQUENTIAL;

        return options;
    }
}
