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
        protected readonly IClock _clock;

        public BaseDbContext(DbContextOptions options, DbSchema dbSchema, IClock clock) : base(options)
        {
            _dbSchema = dbSchema;
            _clock = clock;
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
                entity.AddProperty("UpdatedAt", typeof(DateTimeOffset?));
            }
            foreach (var entity in allEntities.Where(type => type.ClrType.IsAssignableTo(typeof(IRemovable))))
            {
                entity.AddProperty("DeletedAt", typeof(DateTimeOffset?));
            }
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditable>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var today = _clock.Now;

                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = today;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdatedAt").CurrentValue = today;
                }
            }

            foreach (var entry in ChangeTracker.Entries<IRemovable>().Where(e => e.State == EntityState.Deleted))
            {
                var today = _clock.Now;

                entry.Property("DeletedAt").CurrentValue = today;

                entry.State = EntityState.Modified;
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public IEnumerable<IDomainEvent> Get()
        {
            var aggregateRoots = ChangeTracker.Entries<AggregateRoot>().Select(entry => entry.Entity).ToList();

            var events = aggregateRoots.SelectMany(aggregateRoot => aggregateRoot.Events).ToArray();

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

        public async Task<long> GetNextLongValue<T>()
        {
            var result = new SqlParameter("@result", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };

            var query = $"SELECT @result = (NEXT VALUE FOR {typeof(T).Name}_Sequence)";

            if (!string.IsNullOrEmpty(_dbSchema.Name))
            {
                query = $"SELECT @result = (NEXT VALUE FOR [{_dbSchema.Name}].[{typeof(T).Name}_Sequence])";
            }

            await base.Database.ExecuteSqlRawAsync(query, result);

            return (long)result.Value;
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