using System.Net;
using FindLiteAI.Core.Exceptions;

namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Provides model package download functionality.
/// </summary>
public static class ModelDownloadService
{
    /// <summary>
    /// Downloads a FindLiteAI model package ZIP file.
    /// </summary>
    /// <param name="model">
    /// The model definition.
    /// </param>
    /// <param name="targetZipPath">
    /// The target ZIP file path.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The downloaded ZIP file path.
    /// </returns>
    /// <exception cref="SearchException">
    /// Thrown when the model package download fails.
    /// </exception>
    public static async Task<string> DownloadAsync(
        FindLiteAIModelDefinition model,
        string targetZipPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (string.IsNullOrWhiteSpace(model.PackageSource))
        {
            throw new SearchException(
                $"Package source URL is not configured for model '{model.Id}'.");
        }

        if (string.IsNullOrWhiteSpace(targetZipPath))
        {
            throw new SearchException(
                "Target ZIP file path must be configured.");
        }

        string? directory =
            Path.GetDirectoryName(targetZipPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        try
        {
            using HttpClient client = new();

            using HttpResponseMessage response =
                await client.GetAsync(
                    model.PackageSource,
                    cancellationToken);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new SearchException(
                    $"Model package download failed with HTTP status code {(int)response.StatusCode}.");
            }

            await using Stream sourceStream =
                await response.Content.ReadAsStreamAsync(
                    cancellationToken);

            await using FileStream destinationStream =
                File.Create(targetZipPath);

            await sourceStream.CopyToAsync(
                destinationStream,
                cancellationToken);

            return targetZipPath;
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            throw new SearchException(
                $"Failed to download model package '{model.Id}'.",
                exception);
        }
    }
}
