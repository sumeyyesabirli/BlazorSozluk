using BlazorSozluk.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlazorSozluk.Infrastructure.Persistence.Context
{
    public class BlozorSozlukContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "dbo";

        public BlozorSozlukContext()
        {

        }
        public BlozorSozlukContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Entry> Entries { get; set; }

        public DbSet<EntryVote> EntryVotes { get; set; }
        public DbSet<EntryFavorite> EntryFavorites { get; set; }

        public DbSet<EntryComment> EntryComments { get; set; }
        public DbSet<EntryCommentVote> EntryCommentVotes { get; set; }
        public DbSet<EntryCommentFavorite> EntryCommentFavorites { get; set; }

        public DbSet<EmailConfirmation> EmailConfirmations { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connStr = "Server = postgres;User ID=postgres; Password = 12345; Host = localhost; Port = 5432; Database = Blazorsozluk";
                optionsBuilder.UseNpgsql(connStr, opt =>
                {
                    opt.EnableRetryOnFailure();
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp").Entity<User>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            modelBuilder.HasPostgresExtension("uuid-ossp").Entity<Entry>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            modelBuilder.HasPostgresExtension("uuid-ossp").Entity<EntryVote>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            modelBuilder.HasPostgresExtension("uuid-ossp").Entity<EntryFavorite>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            modelBuilder.HasPostgresExtension("uuid-ossp").Entity<EntryComment>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            modelBuilder.HasPostgresExtension("uuid-ossp").Entity<EntryCommentVote>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            modelBuilder.HasPostgresExtension("uuid-ossp").Entity<EntryCommentFavorite>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
        public override int SaveChanges()
        {
            OnBeforeSave();
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(cancellationToken);
        }
        private void OnBeforeSave()
        {
            var addedEntites = ChangeTracker.Entries()
                                    .Where(i => i.State == EntityState.Added)
                                    .Select(i => (BaseEntity)i.Entity);

            PrepareAddedEntities(addedEntites);
        }

        private void PrepareAddedEntities(IEnumerable<BaseEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.CreateDate == DateTime.MinValue)
                    entity.CreateDate = DateTime.UtcNow;
            }
        }
    }
}
