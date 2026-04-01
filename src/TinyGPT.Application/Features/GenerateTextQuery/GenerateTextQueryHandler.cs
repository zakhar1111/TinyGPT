using Microsoft.Extensions.Configuration;
using TinyGPT.Domain.Services;

namespace TinyGPT.Application.Features.GenerateTextQuery;

public sealed class GenerateTextQueryHandler 
    : IOperationHandler<GenerateTextQuery, string>
{
    private readonly IGenerationService _generationService;
    private readonly int _seqLength;

    public GenerateTextQueryHandler(
        IGenerationService generationService,
        IConfiguration config
    )
    {
        _generationService = generationService;
        _seqLength = config.GetValue<int>("ModelConfig:SeqLength"); ;
    }


    public async Task<string?> HandleAsync(GenerateTextQuery request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.StartText))
            throw new ArgumentException("Start text cannot be empty.");

        var result = _generationService.Generate(
            request.StartText,
            request.Length,
            _seqLength // use config
        );

        return await Task.FromResult(result);
    }
}
