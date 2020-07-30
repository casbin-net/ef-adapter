using System;
using System.Data.Entity;

namespace Casbin.NET.Adapter.EF
{
    // code taken from https://stackoverflow.com/a/38126288
    // ref: https://coding.abel.nu/2012/03/prevent-ef-migrations-from-creating-or-changing-the-database/
    public class ValidateDbInitializer<TContext> : IDatabaseInitializer<TContext>
        where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
            if (!context.Database.Exists())
            {
                throw new InvalidOperationException("The database does not exist. Check your server and connection string.");
            }
            if (!context.Database.CompatibleWithModel(true))
            {
                throw new InvalidOperationException("The database is not up to date. You may need to apply update(s).");
            }
        }
    }
}
