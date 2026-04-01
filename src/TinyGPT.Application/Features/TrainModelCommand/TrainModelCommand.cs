using TinyGPT.Infrastructure.Services;

namespace TinyGPT.Application.Features.TrainModelCommand;

public class TrainModelCommand
    : IRequest<string>
{
    public string Text { get; set; } 
    public int Epochs { get; } = 100;
    public float LearningRate { get; } = 0.01f;
}
