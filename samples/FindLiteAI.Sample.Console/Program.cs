using FindLiteAI.Embeddings.Onnx;

const string modelPath =
    @"D:\AIModels\FindLiteAI\all-MiniLM-L6-v2\model.onnx";

const string vocabularyPath =
    @"D:\AIModels\FindLiteAI\all-MiniLM-L6-v2\vocab.txt";

OnnxEmbeddingProvider provider = new(
    new OnnxEmbeddingProviderOptions
    {
        ModelPath = modelPath,
        VocabularyPath = vocabularyPath,
        MaxTokenLength = 256,
        WarmupOnLoad = true
    });

IReadOnlyList<float> embedding =
    await provider.GenerateEmbeddingAsync(
        "SFTP authentication failed for remote user.");

Console.WriteLine("FindLiteAI ONNX MiniLM test");
Console.WriteLine("---------------------------");
Console.WriteLine($"Embedding dimensions: {embedding.Count}");
Console.WriteLine($"First 5 values: {string.Join(", ", embedding.Take(5))}");
