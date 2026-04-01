namespace TinyGPT.Application;

public interface IRequest<TResult> { }

public interface IOperationHandler<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    Task<TResult?> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

public class OperationExecutor(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<TResult?> ExecuteAsync<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest<TResult>
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));
        var handlerObj = _serviceProvider.GetService(typeof(IOperationHandler<TRequest, TResult>));
        if (handlerObj is not IOperationHandler<TRequest, TResult> handler)
            throw new InvalidOperationException(
                $"No handler found for request type {typeof(TRequest).Name}"
            );
        return await handler.HandleAsync(request, cancellationToken);
    }
}