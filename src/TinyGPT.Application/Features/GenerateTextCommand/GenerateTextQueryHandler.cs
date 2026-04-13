using Microsoft.Extensions.Configuration;
using TinyGPT.Domain.Services;

namespace TinyGPT.Application.Features.GenerateTextCommand;

public sealed class GenerateTextQueryHandler 
    : IOperationHandler<GenerateTextCommand, string>
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


    public async Task<string?> HandleAsync(GenerateTextCommand request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            throw new ArgumentException("Start text cannot be empty.");

        var result = await _generationService.GenerateAsync(
            request.Prompt,
            request.MaxTokens,
            _seqLength // use config
        );

        return result;
    }
}
