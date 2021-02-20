using Microsoft.EntityFrameworkCore;

namespace EFSample.Models 
{
    public class AdvDbContext: DbContext 
    {
        public AdvDbContext(DbContextOptions<AdvDbContext> options) : base(options) {}
        
        public DbSet<Product> Products { get; set; }

        public DbSet<ScalarInt> ScalarIntValue { get; set; }

        /// <summary>
        /// Override OnModelCreating event in AdvDbContext class to inform EF that that the ScalarInt class has no primary key.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            //When you use a DbSet<T> property, EF assumes that you're following good relational database practices 
            // such as the use of primary keys. However, the ScalarInt class doesn't correspond to a table 
            // and thus doesn't have a primary key. 
            //It's up to you to tell EF that there's no primary key by adding a line of code in the OnModelCreating() event.
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ScalarInt>().HasNoKey();
        }
    }
}
