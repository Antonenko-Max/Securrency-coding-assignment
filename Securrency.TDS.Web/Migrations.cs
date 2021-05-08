using Securrency.TDS.Web.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace Securrency.TDS.Web
{
    public class Migrations
    {
        private readonly IDbContextFactory _dbContextFactory;

        public Migrations(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Run()
        {
            using AppDbContext ctx = _dbContextFactory.CreateContext();
            ctx.Database.Migrate();
        }
    }
}