using LibraryManagementSystemAimanSahharon.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemAimanSahharon.Data
{
    //to link to Db
    public class LibraryDbContext : DbContext
    {
        // Constructor receives options (connection string etc.) via DI
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        // Each DbSet maps to a table in SQL Server
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Loan> Loans => Set<Loan>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Book configuration ---
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);

                // Unique constraint on ISBN — no two books can share an ISBN
                entity.HasIndex(b => b.ISBN).IsUnique();

                entity.Property(b => b.Title).IsRequired().HasMaxLength(300);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(200);
                entity.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            });

            // --- Member configuration ---
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(m => m.Id);

                // Unique index on SsoSubject — prevents duplicate provisioning
                entity.HasIndex(m => m.SsoSubject).IsUnique();

                // Also index Email for fast lookups
                entity.HasIndex(m => m.Email).IsUnique();

                entity.Property(m => m.Role).IsRequired().HasMaxLength(50)
                      .HasDefaultValue("Member");
            });

            // --- Loan configuration ---
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(l => l.Id);

                // A loan belongs to one book; deleting a book deletes its loans
                entity.HasOne(l => l.Book)
                      .WithMany(b => b.Loans)
                      .HasForeignKey(l => l.BookId)
                      .OnDelete(DeleteBehavior.Cascade);

                // A loan belongs to one member; deleting a member deletes their loans
                entity.HasOne(l => l.Member)
                      .WithMany(m => m.Loans)
                      .HasForeignKey(l => l.MemberId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Index on (MemberId, ReturnedDate) — fast query for active loans per member
                entity.HasIndex(l => new { l.MemberId, l.ReturnedDate });
            });
        }
    }
}
