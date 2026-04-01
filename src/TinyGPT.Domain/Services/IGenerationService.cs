namespace TinyGPT.Domain.Services;

public interface IGenerationService
{
    string Generate(string start, int length, int seqLength);
}
