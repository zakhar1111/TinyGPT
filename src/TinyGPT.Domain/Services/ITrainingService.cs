using TinyGPT.Domain.Entities;

namespace TinyGPT.Domain.Services;

public interface ITrainingService
{
    Task TrainAsync(string text, int epochs = 100, float lr = 0.01f);
}
