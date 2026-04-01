namespace TinyGPT.Application.Features.GenerateTextQuery;

public sealed class GenerateTextQuery : IRequest<string>
{
    public string StartText { get; }
    public int Length { get; }

    public GenerateTextQuery(string startText, int length = 20)
    {
        StartText = startText ?? throw new ArgumentNullException(nameof(startText));
        Length = length;
    }
}
