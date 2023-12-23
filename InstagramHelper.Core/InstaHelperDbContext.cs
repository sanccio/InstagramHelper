using InstagramHelper.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace InstagramHelper.Core
{
    public class InstaHelperDbContext : DbContext
    {
        public DbSet<InstaUser> InstagramUsers { get; set; }
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        
        public InstaHelperDbContext(DbContextOptions<InstaHelperDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);
            modelbuilder.Entity<Subscription>().HasKey(s => new { s.TelegramUserId, s.InstaUsername });
            modelbuilder.Entity<TelegramUser>().Property(u => u.Id).ValueGeneratedNever();
        }
    }
}
