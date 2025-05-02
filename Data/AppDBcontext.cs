
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using GestBurOrdAPI.Models;

namespace GestBurOrdAPI.Data

{
    public class AppDBcontext : DbContext
    {

        public AppDBcontext(DbContextOptions<AppDBcontext> options) : base(options)
        {
        }

        public DbSet<Demande> Demandes { get; set; }

        public DbSet<Documents> Documents { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<User> Users { get; set; }



    }
}
