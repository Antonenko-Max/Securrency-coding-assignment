using System.Transactions;
using Securrency.TDS.Web.DataLayer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Securrency.TDS.Test
{
    public static class Db
    {
        internal static string MigrationsConnectionString => "Server=localhost;Database=Securrency_TDS;User Id=Securrency_TDS_Adm;Password=1QAZ2wsx3EDC;";

        public static string ApplicationConnectionString => "Server=localhost;Database=Securrency_TDS;User Id=Securrency_TDS_App;Password=1QAZ2wsx3EDC;";

        public static AppDbContext GetAdminContext()
        {
            var opts = new OptionsWrapper<DataLayerOptions>(new DataLayerOptions
                {ConnectionString = MigrationsConnectionString});
            return new AppDbContext(opts, null);
        }

        public static void Recreate()
        {
            Drop();
            Migrate();
        }

        private static void Drop()
        {
            using var scope = new TransactionScope(TransactionScopeOption.RequiresNew,
                new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted});

            using var conn = new SqlConnection(MigrationsConnectionString);
            conn.Open();

            var schema = new DropSchema.SqlServer.DropSchema(conn);
            schema.Run(EntityTypeBuilderExtensions.SchemaName);

            scope.Complete();
        }

        private static void Migrate()
        {
            using AppDbContext ctx = GetAdminContext();
            ctx.Database.Migrate();
        }
    }
}