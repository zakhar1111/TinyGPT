namespace TinyGPT.Domain.Repositories;

public interface IEmbeddingRepository
{
    float[,] Embeddings { get; }
    float[,] Positional { get; }

    void Initialize(int vocabSize);
}