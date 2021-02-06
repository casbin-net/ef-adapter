using System.Data.Entity;
using Casbin.NET.Adapter.EF.Model;

namespace Casbin.NET.Adapter.EF
{
    public partial class CasbinDbContext : DbContext 
    {
        public virtual DbSet<CasbinRule> CasbinRule { get; set; }


        public CasbinDbContext(string databaseConnectionString)
            : this(databaseConnectionString, new ValidateDbInitializer<CasbinDbContext>(), false)
        {
        }

        public CasbinDbContext(string databaseConnectionString, IDatabaseInitializer<CasbinDbContext> initializer, bool forceInit = true)
            : base(databaseConnectionString)
        {
            Database.SetInitializer(initializer);
            Database.Initialize(forceInit);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CasbinRule>().Ignore(m => m.Id);
        }

    }
}
