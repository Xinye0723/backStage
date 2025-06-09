using Microsoft.EntityFrameworkCore;

namespace backStage.Models
{
    public partial class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("movie"));
            }
        }
    }
}
