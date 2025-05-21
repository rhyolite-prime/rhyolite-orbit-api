using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace RhyoliteOrbit.EntityFrameworkCore;

public static class RhyoliteOrbitDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<RhyoliteOrbitDbContext> builder, string connectionString)
    {
        builder.UseNpgsql(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<RhyoliteOrbitDbContext> builder, DbConnection connection)
    {
        builder.UseNpgsql(connection);
    }
}
