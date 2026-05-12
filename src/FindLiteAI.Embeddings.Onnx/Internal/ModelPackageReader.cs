using System.Text.Json;
using FindLiteAI.Core.Exceptions;

namespace FindLiteAI.Embeddings.Onnx.Internal;

/// <summary>
/// Provides functionality for reading FindLiteAI model package metadata.
/// </summary>
internal static class ModelPackageReader
{
    private const string ModelInfoFileName = "MODEL_INFO.json";

    /// <summary>
    /// Reads model package metadata from an extracted model package directory.
    /// </summary>
    /// <param name="modelDirectory">
    /// The extracted model package directory.
    /// </param>
    /// <returns>
    /// The parsed model package metadata.
    /// </returns>
    /// <exception cref="SearchException">
    /// Thrown when the model metadata file is missing or invalid.
    /// </exception>
    public static ModelPackageInfo Read(
        string modelDirectory)
    {
        if (string.IsNullOrWhiteSpace(modelDirectory))
        {
            throw new SearchException(
                "Model directory must be configured.");
        }

        if (!Directory.Exists(modelDirectory))
        {
            throw new SearchException(
                $"Model directory does not exist: '{modelDirectory}'.");
        }

        string modelInfoPath =
            Path.Combine(
                modelDirectory,
                ModelInfoFileName);

        if (!File.Exists(modelInfoPath))
        {
            throw new SearchException(
                $"Model metadata file was not found: '{modelInfoPath}'.");
        }

        try
        {
            string json =
                File.ReadAllText(modelInfoPath);

            ModelPackageInfo? modelInfo =
                JsonSerializer.Deserialize<ModelPackageInfo>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (modelInfo is null)
            {
                throw new SearchException(
                    $"Model metadata file is empty or invalid: '{modelInfoPath}'.");
            }

            Validate(modelInfo, modelInfoPath);

            return modelInfo;
        }
        catch (JsonException exception)
        {
            throw new SearchException(
                $"Failed to parse model metadata file: '{modelInfoPath}'.",
                exception);
        }
    }

    private static void Validate(
        ModelPackageInfo modelInfo,
        string modelInfoPath)
    {
        if (string.IsNullOrWhiteSpace(modelInfo.Id))
        {
            throw new SearchException(
                $"Model metadata is missing id in '{modelInfoPath}'.");
        }

        if (string.IsNullOrWhiteSpace(modelInfo.DisplayName))
        {
            throw new SearchException(
                $"Model metadata is missing displayName in '{modelInfoPath}'.");
        }

        if (string.IsNullOrWhiteSpace(modelInfo.OnnxPath))
        {
            throw new SearchException(
                $"Model metadata is missing onnxPath in '{modelInfoPath}'.");
        }

        if (modelInfo.Dimensions <= 0)
        {
            throw new SearchException(
                $"Model metadata has invalid dimensions in '{modelInfoPath}'.");
        }
    }
}
