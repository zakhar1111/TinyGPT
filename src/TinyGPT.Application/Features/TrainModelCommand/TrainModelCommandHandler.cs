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
        if (string.IsNullOrWhiteSpace(request.Text))
            throw new ArgumentException("Training text cannot be empty.");

        // ================= DOMAIN ORCHESTRATION =================
        // This is the CORRECT place to call training
        await _trainingService.TrainAsync(
            request.Text,
            request.Epochs,
            request.LearningRate
        );

        // ================= RESULT =================
        return "Model training completed successfully.";
    }
}