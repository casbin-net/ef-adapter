using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System;
using Casbin.NET.Adapter.EF.Model;

namespace Casbin.NET.Adapter.EF
{
    public partial class CasbinDbContext<TKey> : DbContext where TKey : IEquatable<TKey>
    {
        public virtual DbSet<CasbinRule<TKey>> CasbinRule { get; set; }


        public CasbinDbContext(string databaseConnectionString)
            : this(databaseConnectionString, new ValidateDbInitializer<CasbinDbContext<TKey>>(), false)
        {
        }

        public CasbinDbContext(string databaseConnectionString, EntityTypeConfiguration<CasbinDbContext<TKey>> entityType)
            : this(databaseConnectionString, new ValidateDbInitializer<CasbinDbContext<TKey>>(), false)
        {
        }

        public CasbinDbContext(string databaseConnectionString, IDatabaseInitializer<CasbinDbContext<TKey>> initializer, bool forceInit = true)
            : base(databaseConnectionString)
        {
            Database.SetInitializer(initializer);
            Database.Initialize(forceInit);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CasbinRule<TKey>>();
        }

    }
}
