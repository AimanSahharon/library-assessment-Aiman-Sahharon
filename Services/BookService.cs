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
            string? authorFilter, string? titleFilter, string? sortBy)
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

            query = sortBy switch
            {
                "title_asc" => query.OrderBy(b => b.Title),
                "title_desc" => query.OrderByDescending(b => b.Title),

                "author_asc" => query.OrderBy(b => b.Author),
                "author_desc" => query.OrderByDescending(b => b.Author),

                _ => query.OrderBy(b => b.Title) // default
            };


            //return await query.OrderBy(b => b.Title).ToListAsync();
            return await query.ToListAsync();
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
            var exists = await _db.Books
        .AnyAsync(b => b.ISBN == book.ISBN);

            //if (exists)
            //{
            //    _logger.LogWarning("Duplicate ISBN attempted: {ISBN}", book.ISBN);
            //    throw new InvalidOperationException("A book with this ISBN already exists.");
            //}

            //if (book.TotalCopies <= 0)
            //    throw new InvalidOperationException("Total copies must be greater than 0.");

            //if (book.PublishedYear > DateTime.Now.Year)
            //    throw new InvalidOperationException("Published year cannot be in the future.");

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

        public async Task<bool> IsIsbnExistsAsync(string isbn) //check if there is any existing isbn
        {
            return await _db.Books.AnyAsync(b => b.ISBN == isbn);
        }
    }
}
