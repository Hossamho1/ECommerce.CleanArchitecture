using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Data.Interceptors
{
    public sealed class AuditColumnsInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyAuditColumns(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAuditColumns(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ApplyAuditColumns(DbContext? context)
        {
            if (context is null)
                return;

            var utcNow = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = utcNow;
                        entry.Entity.UpdatedAt = null;

                        if (entry.Entity.IsDeleted && entry.Entity.DeletedAt is null)
                        {
                            entry.Entity.DeletedAt = utcNow;
                        }
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = utcNow;

                        if (entry.Entity.IsDeleted)
                        {
                            if (entry.Entity.DeletedAt is null)
                                entry.Entity.DeletedAt = utcNow;
                        }
                        else
                        {
                            entry.Entity.DeletedAt = null;
                        }
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; 
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = utcNow;
                        break;
                }
            }
        }
    }
}