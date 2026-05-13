using FindLiteAI.Core.Exceptions;

namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Provides model package installation functionality.
/// </summary>
public static class ModelInstallService
{
    /// <summary>
    /// Downloads and extracts a FindLiteAI model package into the user model cache.
    /// </summary>
    /// <param name="model">
    /// The model definition to install.
    /// </param>
    /// <param name="overwrite">
    /// A value indicating whether an existing extracted model package should be overwritten.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The extracted model directory containing MODEL_INFO.json.
    /// </returns>
    /// <exception cref="SearchException">
    /// Thrown when model installation fails.
    /// </exception>
    public static async Task<string> InstallAsync(
        FindLiteAIModelDefinition model,
        bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        string cacheDirectory =
            ModelCachePaths.GetUserCacheDirectory();

        return await InstallAsync(
            model,
            cacheDirectory,
            overwrite,
            cancellationToken);
    }

    /// <summary>
    /// Downloads and extracts a FindLiteAI model package into the specified cache directory.
    /// </summary>
    /// <param name="model">
    /// The model definition to install.
    /// </param>
    /// <param name="cacheDirectory">
    /// The cache directory.
    /// </param>
    /// <param name="overwrite">
    /// A value indicating whether an existing extracted model package should be overwritten.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The extracted model directory containing MODEL_INFO.json.
    /// </returns>
    /// <exception cref="SearchException">
    /// Thrown when model installation fails.
    /// </exception>
    public static async Task<string> InstallAsync(
        FindLiteAIModelDefinition model,
        string cacheDirectory,
        bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (string.IsNullOrWhiteSpace(cacheDirectory))
        {
            throw new SearchException(
                "Model cache directory must be configured.");
        }

        Directory.CreateDirectory(cacheDirectory);

        string finalModelDirectory =
            Path.Combine(
                cacheDirectory,
                model.Id);

        if (Directory.Exists(finalModelDirectory) &&
            !overwrite &&
            ContainsModelInfo(finalModelDirectory))
        {
            return finalModelDirectory;
        }

        string zipPath =
            Path.Combine(
                cacheDirectory,
                $"{model.Id}.zip");

        string tempExtractionDirectory =
            Path.Combine(
                cacheDirectory,
                $".extract-{model.Id}-{Guid.NewGuid():N}");

        try
        {
            await ModelDownloadService.DownloadAsync(
                model,
                zipPath,
                cancellationToken);

            string extractedModelDirectory =
                ModelPackageExtractor.Extract(
                    zipPath,
                    tempExtractionDirectory,
                    overwrite: true);

            if (Directory.Exists(finalModelDirectory))
            {
                Directory.Delete(
                    finalModelDirectory,
                    recursive: true);
            }

            Directory.Move(
                extractedModelDirectory,
                finalModelDirectory);

            return finalModelDirectory;
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            throw new SearchException(
                $"Failed to install model package '{model.Id}'.",
                exception);
        }
        finally
        {
            if (Directory.Exists(tempExtractionDirectory))
            {
                Directory.Delete(
                    tempExtractionDirectory,
                    recursive: true);
            }
        }
    }

    private static bool ContainsModelInfo(
        string directory)
    {
        return File.Exists(
            Path.Combine(
                directory,
                "MODEL_INFO.json"));
    }
}
