using System.Data;
using ConduitApp.Domain.UserAggregate;
using ConduitApp.Infrastructure.Data.EntityConfigurations;
using ConduitApp.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ConduitApp.Infrastructure;


public class ConduitAppContext : DbContext, IUnitOfWork
{
    private readonly IMediator? _mediator;
    private IDbContextTransaction? _currentTransaction;

    public ConduitAppContext(DbContextOptions<ConduitAppContext> options)
        : base(options)
    {
    }

    public ConduitAppContext(DbContextOptions<ConduitAppContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        System.Diagnostics.Debug.WriteLine("ConduitAppContext::ctor ->" + GetHashCode());
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<FollowedUser> UserLinks { get; set; }

    public bool HasActiveTransaction => _currentTransaction != null;

    public IDbContextTransaction? GetCurrentTransaction()
    {
        return _currentTransaction;
    }

    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            return null;
        }

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        if (transaction != _currentTransaction)
        {
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
        }

        try
        {
            await SaveChangesAsync();
            transaction.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch Domain Events collection.
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.
        if (_mediator != null)
        {
            await _mediator.DispatchDomainEventsAsync(this);
        }

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
        // performed through the DbContext will be committed
        return (await base.SaveChangesAsync(cancellationToken)) > 0;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FollowedUserConfiguration());
    }
}
