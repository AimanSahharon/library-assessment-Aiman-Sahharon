using LibraryManagementSystemAimanSahharon.Models;

namespace LibraryManagementSystemAimanSahharon.Services
{
        // Interface for book service to manage book-related operations
        public interface IBookService
        {
            Task<IEnumerable<Book>> GetAllBooksAsync(string? authorFilter, string? titleFilter);
            Task<Book?> GetBookByIdAsync(int id);
            Task<Book> CreateBookAsync(Book book);
            Task<Book?> UpdateBookAsync(int id, Book updated);
            Task<bool> DeleteBookAsync(int id);
        }
}
