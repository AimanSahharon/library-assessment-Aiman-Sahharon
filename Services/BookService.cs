using LibraryManagementSystemAimanSahharon.Data;
using LibraryManagementSystemAimanSahharon.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemAimanSahharon.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _db;
        private readonly ILogger<BookService> _logger;

        // EF DbContext and ILogger injected by the DI container
        public BookService(LibraryDbContext db, ILogger<BookService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(
            string? authorFilter, string? titleFilter)
        {
            // Start with all books, including their loans so we can count available copies
            var query = _db.Books.Include(b => b.Loans).AsQueryable();

            // Case-insensitive partial match on Author
            if (!string.IsNullOrWhiteSpace(authorFilter))
                query = query.Where(b =>
                    b.Author.ToLower().Contains(authorFilter.ToLower()));

            // Case-insensitive partial match on Title
            if (!string.IsNullOrWhiteSpace(titleFilter))
                query = query.Where(b =>
                    b.Title.ToLower().Contains(titleFilter.ToLower()));

            return await query.OrderBy(b => b.Title).ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            // Include Loans so AvailableCopies computed property works correctly
            return await _db.Books
                .Include(b => b.Loans)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Book created: {Title} (ISBN: {ISBN})", book.Title, book.ISBN);
            return book;
        }

        public async Task<Book?> UpdateBookAsync(int id, Book updated)
        {
            var existing = await _db.Books.FindAsync(id);
            if (existing == null) return null;

            // Map updated fields onto the tracked entity
            existing.Title = updated.Title;
            existing.Author = updated.Author;
            existing.ISBN = updated.ISBN;
            existing.PublishedYear = updated.PublishedYear;
            existing.TotalCopies = updated.TotalCopies;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Book updated: ID {Id}", id);
            return existing;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return false;

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Book deleted: ID {Id}", id);
            return true;
        }
    }
}
