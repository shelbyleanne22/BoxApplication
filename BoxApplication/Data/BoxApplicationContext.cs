using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;

namespace BoxApplication.Models
{
    public class BoxApplicationContext : DbContext
    {
        public BoxApplicationContext (DbContextOptions<BoxApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<BoxApplication.Models.ActiveDirectoryUser> ActiveDirectoryUsers { get; set; }

        public DbSet<BoxApplication.Models.ApplicationAction> ApplicationActions { get; set; }

        public DbSet<BoxApplication.Models.BoxUsers> BoxUsers { get; set; }

        public DbSet<BoxApplication.Models.BoxFile> BoxFile { get; set; }
    }
}
