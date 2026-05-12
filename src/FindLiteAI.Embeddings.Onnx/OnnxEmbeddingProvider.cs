using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Exceptions;
using FindLiteAI.Embeddings.Onnx.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Provides ONNX-based semantic embedding generation.
/// </summary>
public sealed class OnnxEmbeddingProvider : IEmbeddingProvider, IDisposable
{
    private readonly InferenceSession _session;
    private readonly TokenizerService _tokenizerService;
    private readonly ILogger<OnnxEmbeddingProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnnxEmbeddingProvider"/> class.
    /// </summary>
    /// <param name="options">The ONNX embedding provider configuration.</param>
    /// <param name="logger">The logger instance.</param>
    public OnnxEmbeddingProvider(
    OnnxEmbeddingProviderOptions options,
    ILogger<OnnxEmbeddingProvider> logger)
    {
        ArgumentNullException.ThrowIfNull(options);

        OnnxOptionsValidator.Validate(options);

        _logger = logger;

        _logger.LogInformation(
            "Loading ONNX embedding model from '{ModelPath}'.",
            options.ModelPath);

        _session = ModelLoader.LoadSession(options);

        _tokenizerService = new TokenizerService(options);

        _logger.LogInformation(
            "ONNX embedding model loaded successfully.");

        if (options.WarmupOnLoad)
        {
            _logger.LogDebug("Warming up ONNX embedding provider.");

            Warmup();

            _logger.LogDebug("ONNX embedding provider warmup completed.");
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<float>> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        try
        {
            TokenizedInput tokenizedInput =
                _tokenizerService.Tokenize(text);

            DenseTensor<long> inputIdsTensor =
                CreateTensor(tokenizedInput.InputIds);

            DenseTensor<long> attentionMaskTensor =
                CreateTensor(tokenizedInput.AttentionMask);

            DenseTensor<long> tokenTypeIdsTensor =
                CreateTensor(tokenizedInput.TokenTypeIds);

            List<NamedOnnxValue> inputs =
            [
                NamedOnnxValue.CreateFromTensor(
                    "input_ids",
                    inputIdsTensor),

                NamedOnnxValue.CreateFromTensor(
                    "attention_mask",
                    attentionMaskTensor),

                NamedOnnxValue.CreateFromTensor(
                    "token_type_ids",
                    tokenTypeIdsTensor)
            ];

            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results =
                _session.Run(inputs);

            DisposableNamedOnnxValue output =
                results.First();

            Tensor<float> outputTensor =
                output.AsTensor<float>();

            float[] embedding =
                MeanPool(outputTensor, tokenizedInput.Length);

            _logger.LogDebug(
                "Generated embedding with {DimensionCount} dimensions.",
                embedding.Length);

            return Task.FromResult<IReadOnlyList<float>>(embedding);
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to generate ONNX embedding.");

            throw new SearchException(
                "Failed to generate ONNX embedding.",
                exception);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        IReadOnlyCollection<string> texts,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(texts);

            _logger.LogDebug(
                "Generating embeddings for {TextCount} texts.",
                texts.Count);

            List<IReadOnlyList<float>> embeddings = [];

            foreach (string text in texts)
            {
                IReadOnlyList<float> embedding =
                    await GenerateEmbeddingAsync(
                        text,
                        cancellationToken);

                embeddings.Add(embedding);
            }

            _logger.LogDebug(
                "Generated {EmbeddingCount} embeddings.",
                embeddings.Count);

            return embeddings;
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to generate ONNX embeddings.");

            throw new SearchException(
                "Failed to generate ONNX embeddings.",
                exception);
        }
    }

    /// <summary>
    /// Releases ONNX Runtime resources.
    /// </summary>
    public void Dispose()
    {
        _session.Dispose();

        _logger.LogDebug("Disposed ONNX embedding provider.");
    }

    private static DenseTensor<long> CreateTensor(
        IReadOnlyList<long> values)
    {
        return new DenseTensor<long>(
            values.ToArray(),
            [1, values.Count]);
    }

    private static float[] MeanPool(
        Tensor<float> tensor,
        int tokenCount)
    {
        int embeddingSize = tensor.Dimensions[2];

        float[] pooled = new float[embeddingSize];

        for (int tokenIndex = 0;
             tokenIndex < tokenCount;
             tokenIndex++)
        {
            for (int embeddingIndex = 0;
                 embeddingIndex < embeddingSize;
                 embeddingIndex++)
            {
                pooled[embeddingIndex] +=
                    tensor[0, tokenIndex, embeddingIndex];
            }
        }

        for (int embeddingIndex = 0;
             embeddingIndex < embeddingSize;
             embeddingIndex++)
        {
            pooled[embeddingIndex] /= tokenCount;
        }

        return pooled;
    }

    private void Warmup()
    {
        GenerateEmbeddingAsync("warmup")
            .GetAwaiter()
            .GetResult();
    }
}
