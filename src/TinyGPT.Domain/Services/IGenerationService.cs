using System.Runtime.CompilerServices;

namespace TinyGPT.Domain.Services;

public interface IGenerationService
{
    Task<string> GenerateAsync(
        string start, 
        int length, 
        int seqLength,
        CancellationToken ct = default);
    IAsyncEnumerable<string> GenerateStreamAsync(
        string start,
        int length,
        int seqLength,
        [EnumeratorCancellation] CancellationToken ct);
}
