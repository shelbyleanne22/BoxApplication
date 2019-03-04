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

        public DbSet<BoxApplication.Models.ActiveDirectoryUser> ActiveDirectoryUser { get; set; }

        public DbSet<BoxApplication.Models.ApplicationAction> Action { get; set; }

        public DbSet<BoxApplication.Models.BoxUsers> BoxUsers { get; set; }
    }
}
