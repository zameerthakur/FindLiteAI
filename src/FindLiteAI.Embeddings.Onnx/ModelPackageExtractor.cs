using System.IO.Compression;
using FindLiteAI.Core.Exceptions;

namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Provides model package extraction functionality.
/// </summary>
public static class ModelPackageExtractor
{
    /// <summary>
    /// Extracts a FindLiteAI model package ZIP file to a target directory.
    /// </summary>
    /// <param name="zipPath">
    /// The model package ZIP file path.
    /// </param>
    /// <param name="targetDirectory">
    /// The directory where the package should be extracted.
    /// </param>
    /// <param name="overwrite">
    /// A value indicating whether an existing target directory should be overwritten.
    /// </param>
    /// <returns>
    /// The extracted model directory containing MODEL_INFO.json.
    /// </returns>
    /// <exception cref="SearchException">
    /// Thrown when the ZIP package cannot be extracted or resolved.
    /// </exception>
    public static string Extract(
        string zipPath,
        string targetDirectory,
        bool overwrite = false)
    {
        ValidateInputs(
            zipPath,
            targetDirectory);

        try
        {
            if (Directory.Exists(targetDirectory))
            {
                if (!overwrite)
                {
                    return ResolveModelDirectory(targetDirectory);
                }

                Directory.Delete(
                    targetDirectory,
                    recursive: true);
            }

            Directory.CreateDirectory(targetDirectory);

            ZipFile.ExtractToDirectory(
                zipPath,
                targetDirectory,
                overwriteFiles: true);

            return ResolveModelDirectory(targetDirectory);
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            throw new SearchException(
                $"Failed to extract model ZIP package '{zipPath}' to '{targetDirectory}'.",
                exception);
        }
    }

    private static void ValidateInputs(
        string zipPath,
        string targetDirectory)
    {
        if (string.IsNullOrWhiteSpace(zipPath))
        {
            throw new SearchException(
                "Model ZIP path must be configured.");
        }

        if (!File.Exists(zipPath))
        {
            throw new SearchException(
                $"Model ZIP file was not found: '{zipPath}'.");
        }

        if (!Path.GetExtension(zipPath)
                .Equals(".zip", StringComparison.OrdinalIgnoreCase))
        {
            throw new SearchException(
                $"Model package must be a .zip file. Provided path: '{zipPath}'.");
        }

        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            throw new SearchException(
                "Model extraction target directory must be configured.");
        }
    }

    private static string ResolveModelDirectory(
        string extractionDirectory)
    {
        string directModelInfoPath =
            Path.Combine(
                extractionDirectory,
                "MODEL_INFO.json");

        if (File.Exists(directModelInfoPath))
        {
            return extractionDirectory;
        }

        string[] childDirectories =
            Directory.GetDirectories(extractionDirectory);

        string[] modelDirectories =
            childDirectories
                .Where(directory =>
                    File.Exists(
                        Path.Combine(
                            directory,
                            "MODEL_INFO.json")))
                .ToArray();

        if (modelDirectories.Length == 1)
        {
            return modelDirectories[0];
        }

        if (modelDirectories.Length > 1)
        {
            throw new SearchException(
                $"Multiple model directories containing MODEL_INFO.json were found in '{extractionDirectory}'.");
        }

        throw new SearchException(
            $"No model directory containing MODEL_INFO.json was found in '{extractionDirectory}'.");
    }
}
