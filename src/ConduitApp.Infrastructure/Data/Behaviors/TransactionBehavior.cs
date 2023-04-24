using ConduitApp.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace ConduitApp.Infrastructure;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly ConduitAppContext _dbContext;

    public TransactionBehavior(
        ConduitAppContext dbContext,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentException(nameof(ConduitAppContext));
        _logger = logger ?? throw new ArgumentException(nameof(ILogger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = default(TResponse);
        var typeName = request.GetTypeName();

        try
        {
            if (_dbContext.HasActiveTransaction)
            {
                return await next();
            }

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _dbContext.BeginTransactionAsync())
                using (LogContext.PushProperty("TransactionContext", transaction?.TransactionId))
                {
                    _logger.LogDebug("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction?.TransactionId, typeName, request);

                    response = await next();

                    _logger.LogDebug("----- Commit transaction {TransactionId} for {CommandName}", transaction?.TransactionId, typeName);

                    await _dbContext.CommitTransactionAsync(transaction!);
                }
            });

            return response!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

            throw;
        }
    }
}
