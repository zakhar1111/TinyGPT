namespace TinyGPT.Domain.Services;

public interface IGenerationService
{
    Task<string> GenerateAsync(string start, int length, int seqLength);
}
