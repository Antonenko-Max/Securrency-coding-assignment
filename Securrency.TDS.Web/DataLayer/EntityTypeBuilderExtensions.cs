using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Securrency.TDS.Web.DataLayer
{
    public static class EntityTypeBuilderExtensions
    {
        public static readonly string SchemaName = "Tds";

        public static void ToTableInSchema(this EntityTypeBuilder builder, string table)
        {
            builder.ToTable(table, SchemaName);
        }
    }
}
