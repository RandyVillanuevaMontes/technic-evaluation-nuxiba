namespace Evaluation_Nuxiba.Data
{
    using Evaluation_Nuxiba.Models;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CcLogLogin> CcLogLogins { get; set; }
        public DbSet<CcUser> CcUsers { get; set; }
        public DbSet<ccRIACat_Areas> ccRIACat_Areas { get; set; } = null!;

    }
}
