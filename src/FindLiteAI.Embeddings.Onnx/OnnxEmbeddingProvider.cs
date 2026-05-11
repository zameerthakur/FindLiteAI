using FindLiteAI.Core.Abstractions;
using FindLiteAI.Embeddings.Onnx.Internal;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="OnnxEmbeddingProvider"/> class.
    /// </summary>
    /// <param name="options">
    /// The ONNX embedding provider configuration.
    /// </param>
    public OnnxEmbeddingProvider(
        OnnxEmbeddingProviderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _session = ModelLoader.LoadSession(options);

        _tokenizerService = new TokenizerService(options);

        if (options.WarmupOnLoad)
        {
            Warmup();
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<float>> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
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

        return Task.FromResult<IReadOnlyList<float>>(embedding);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        IReadOnlyCollection<string> texts,
        CancellationToken cancellationToken = default)
    {
        List<IReadOnlyList<float>> embeddings = [];

        foreach (string text in texts)
        {
            IReadOnlyList<float> embedding =
                await GenerateEmbeddingAsync(
                    text,
                    cancellationToken);

            embeddings.Add(embedding);
        }

        return embeddings;
    }

    /// <summary>
    /// Releases ONNX Runtime resources.
    /// </summary>
    public void Dispose()
    {
        _session.Dispose();
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
