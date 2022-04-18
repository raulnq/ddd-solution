using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure
{
    public class BaseDbContext : DbContext, IUnitOfWork, ISequence, IDomainEventSource
    {
        protected readonly DbSchema _dbSchema;

        public BaseDbContext(DbContextOptions options, DbSchema dbSchema) : base(options)
        {
            _dbSchema = dbSchema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!string.IsNullOrEmpty(_dbSchema.Name))
            {
                modelBuilder.HasDefaultSchema(_dbSchema.Name);
            }
            var allEntities = modelBuilder.Model.GetEntityTypes();
            foreach (var entity in allEntities.Where(type => type.ClrType.IsAssignableTo(typeof(IAuditable))))
            {
                entity.AddProperty("CreatedAt", typeof(DateTimeOffset));
                entity.AddProperty("ModifiedAt", typeof(DateTimeOffset));
                entity.AddProperty("ModifiedBy", typeof(string));
                entity.AddProperty("CreatedBy", typeof(string));
            }
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditable>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var today = DateTimeOffset.UtcNow;
                //TODO: Get User Name
                var user = "";

                entry.Property("ModifiedAt").CurrentValue = today;
                entry.Property("ModifiedBy").CurrentValue = user;

                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = today;
                    entry.Property("CreatedBy").CurrentValue = user;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public IEnumerable<IDomainEvent> Get()
        {
            var aggregateRoots = ChangeTracker.Entries<AggregateRoot>().Select(entry => entry.Entity).ToList();

            var events = aggregateRoots.SelectMany(aggregateRoot => aggregateRoot.Events);

            foreach (var aggregateRoot in aggregateRoots)
            {
                aggregateRoot.ClearEvents();
            }

            return events;
        }

        public async Task<int> GetNextValue<T>()
        {
            var result = new SqlParameter("@result", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var query = $"SELECT @result = (NEXT VALUE FOR {typeof(T).Name}_Sequence)";

            if (!string.IsNullOrEmpty(_dbSchema.Name))
            {
                query = $"SELECT @result = (NEXT VALUE FOR [{_dbSchema.Name}].[{typeof(T).Name}_Sequence])";
            }

            await base.Database.ExecuteSqlRawAsync(query, result);

            return (int)result.Value;
        }

        public IDbContextTransaction? CurrentTransaction { get; private set; }

        public async Task BeginTransaction()
        {
            if (CurrentTransaction != null)
            {
                return;
            }

            CurrentTransaction = await base.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }

        public async Task Rollback()
        {
            if (CurrentTransaction == null)
            {
                return;
            }

            try
            {
                await CurrentTransaction.RollbackAsync();
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        public async Task Commit()
        {
            if (CurrentTransaction == null)
            {
                return;
            }
            try
            {
                await SaveChangesAsync();

                CurrentTransaction.Commit();
            }
            catch
            {
                await Rollback();
                throw;
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        public bool IsTransactionOpened()
        {
            return CurrentTransaction != null;
        }
    }
}