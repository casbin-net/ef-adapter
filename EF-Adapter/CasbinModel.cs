using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using Casbin.NET.Adapter.EF.Model;

namespace Casbin.NET.Adapter.EF
{
    public partial class CasbinDbContext<TKey> : DbContext where TKey : IEquatable<TKey>
    {
        private readonly ConfigurationRegistrar _CasbinModelConfig;
        public virtual DbSet<CasbinRule<TKey>> CasbinRule { get; set; }
        

        public CasbinDbContext(string databaseConnectionString)
            : this(databaseConnectionString, new ValidateDbInitializer<CasbinDbContext<TKey>>(), false)
        {
        }

        public CasbinDbContext(string databaseConnectionString, EntityTypeConfiguration<CasbinDbContext<TKey>> entityType) 
            : this (databaseConnectionString, new ValidateDbInitializer<CasbinDbContext<TKey>>(), false)
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
            if (_CasbinModelConfig != null)
            {
                modelBuilder.Entity<CasbinRule<TKey>>();
            }
        }

    }
}
