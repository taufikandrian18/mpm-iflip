using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MPM.FLP.Configuration;
using MPM.FLP.Web;

namespace MPM.FLP.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class FLPDbContextFactory : IDesignTimeDbContextFactory<FLPDbContext>
    {
        public FLPDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FLPDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            FLPDbContextConfigurer.Configure(builder, configuration.GetConnectionString(FLPConsts.ConnectionStringName));

            return new FLPDbContext(builder.Options);
        }
    }
}
