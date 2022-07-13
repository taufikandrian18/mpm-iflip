using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace MPM.FLP.EntityFrameworkCore
{
    public static class FLPDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<FLPDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<FLPDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
