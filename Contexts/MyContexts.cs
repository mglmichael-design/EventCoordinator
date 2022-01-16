using BeltExam.Models;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Contexts
{
    public class MyContexts : DbContext
    {
        public MyContexts(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<FunEvent> FunEvents { get; set; }
        public DbSet<Rsvp> Rsvps { get; set; }

    }
}