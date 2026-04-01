using TinyGPT.Domain.Repositories;
using TinyGPT.Domain.Helpers;

namespace TinyGPT.Infrastructure.Persistemce.Repositories;

public class EmbeddingRepository(int seqLength, int embedSize, Random rnd)
    : IEmbeddingRepository
{
    private readonly  int _seqLength = seqLength;
    private readonly  int _embedSize = embedSize;
    private readonly Random _rnd = rnd;

    public float[,] Embeddings { get; private set; }
    public float[,] Positional { get; private set; }

    public void Initialize(int vocabSize)
    {
        Embeddings = MatrixOperations.RandomMatrix(vocabSize, _embedSize, _rnd);
        Positional = MatrixOperations.RandomMatrix(_seqLength, _embedSize, _rnd);
    }
}
