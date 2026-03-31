using TinyGPT.Domain.Repositories;
using TinyGPT.Domain.Helpers;

namespace TinyGPT.Infrastructure.Persistemce.Repositories;

public class EmbeddingRepository : IEmbeddingRepository
{
    public float[,] Embeddings { get; }
    public float[,] Positional { get; }

    public EmbeddingRepository(int vocabSize, int seqLength, int embedSize, Random rnd)
    {
        Embeddings = MatrixOperations.RandomMatrix(vocabSize, embedSize, rnd);
        Positional = MatrixOperations.RandomMatrix(seqLength, embedSize, rnd);
    }
}
