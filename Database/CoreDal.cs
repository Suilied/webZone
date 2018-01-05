using Microsoft.EntityFrameworkCore;
using webZone.Database.Models;

namespace webZone.Database
{
    public class CoreDal : DbContext
    {
        private static DbContextOptions<CoreDal> _options;

        public DbSet<RotideSettings> rotideSettings { get; set; }

        public CoreDal(DbContextOptions<CoreDal> options) : base(options) { _options = options; }

        protected override void OnModelCreating(ModelBuilder builder){
            builder.Entity<RotideSettings>().HasKey(mn => mn.rotideSettingsId);

            base.OnModelCreating(builder);
        }

        public static CoreDal Create(){
            if(_options == null){
                string connectionString = Global.Configuration["Database:SqlConnectionString"];
                DbContextOptionsBuilder<CoreDal> newOptions = new DbContextOptionsBuilder<CoreDal>();
                newOptions.UseSqlServer(connectionString);
                return new CoreDal(newOptions.Options);
            }
            return new CoreDal(_options);
        }

    }
}
