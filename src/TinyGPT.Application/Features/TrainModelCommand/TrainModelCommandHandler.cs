using TinyGPT.Domain.Services;

namespace TinyGPT.Application.Features.TrainModelCommand;

public class TrainModelCommandHandler(ITrainingService trainingService)
    : IOperationHandler<TrainModelCommand, string>
{
    private readonly ITrainingService _trainingService = trainingService;
    public async Task<string> HandleAsync(
        TrainModelCommand request, 
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.FilePath))
            throw new ArgumentException("File path cannot be empty.");

        if (!File.Exists(request.FilePath))
            throw new FileNotFoundException("Training file not found.", request.FilePath);

        var text = await File.ReadAllTextAsync(request.FilePath, ct);

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Training text cannot be empty.");

        // ================= DOMAIN ORCHESTRATION =================
        // This is the CORRECT place to call training
        await _trainingService.TrainAsync(
            text,
            request.Epochs,
            request.LearningRate,
            ct
        );

        // ================= RESULT =================
        return "Model training completed successfully.";
    }
}