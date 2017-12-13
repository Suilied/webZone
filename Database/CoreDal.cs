using System;
using Microsoft.EntityFrameworkCore;
using webZone.Database.Models;

namespace webZone.Database
{
    public class CoreDal : DbContext
    {
        // Connectionstring
        // "server=146.185.158.204;userid=azmo;pwd=furioso;port=3306;database=webZone;sslmode=none;"

        public DbSet<RotideSettings> rotideSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
            optionsBuilder.UseSqlServer("server=146.185.158.204;userid=azmo;pwd=furioso;port=3306;database=webZone;sslmode=none;");
        }

    }
}
