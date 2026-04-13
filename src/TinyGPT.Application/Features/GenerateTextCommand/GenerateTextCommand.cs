using System.ComponentModel.DataAnnotations;

namespace TinyGPT.Application.Features.GenerateTextCommand;

public class GenerateTextCommand 
    : IRequest<string>
{
    public string Prompt { get; set; } = default!;
    public int MaxTokens { get; set; } = 20;

}
