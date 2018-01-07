using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webZone.Database.Models;

namespace webZone.Database
{
    public class PsqlDal : DbContext
    {
        private static DbContextOptions<PsqlDal> _options;

        public DbSet<Project> projects { get; set; }
        public DbSet<ProjectFile> projectFiles { get; set; }

        public PsqlDal(DbContextOptions<PsqlDal> options) : base(options) { _options = options; }

        protected override void OnConfiguring(DbContextOptionsBuilder options){
            options.UseNpgsql(Global.Configuration["Database:PsqlConnectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Project>().HasKey(m => m.projectId);
            builder.Entity<ProjectFile>().HasKey(m => m.projectFileId);

            base.OnModelCreating(builder);
        }

        // creates localized access to the DB for use outside of the controllers
        public static PsqlDal Create()
        {
            if (_options == null)
            {
                string connectionString = Global.Configuration["Database:PsqlConnectionString"];
                DbContextOptionsBuilder<PsqlDal> newOptions = new DbContextOptionsBuilder<PsqlDal>();
                newOptions.UseNpgsql(connectionString);
                return new PsqlDal(newOptions.Options);
            }
            return new PsqlDal(_options);
        }

    } // PsqlDal
}