using LibraryManagementSystemAimanSahharon.Data;
using LibraryManagementSystemAimanSahharon.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemAimanSahharon.Services
{
    public class LoanService : ILoanService
    {
        private readonly LibraryDbContext _db;
        private readonly ILogger<LoanService> _logger;

        public LoanService(LibraryDbContext db, ILogger<LoanService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, Loan? Loan)>
            BorrowBookAsync(int bookId, int memberId)
        {
            // Load the book with all its active loans to calculate available copies
            var book = await _db.Books
                .Include(b => b.Loans.Where(l => l.ReturnedDate == null))
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
                return (false, "Book not found.", null);

            // Business rule 1: Must have at least one copy available
            if (book.AvailableCopies <= 0)
                return (false, "No copies available for borrowing.", null);

            // Business rule 2: Member may not have 3 or more active loans
            var activeLoansCount = await _db.Loans
                .CountAsync(l => l.MemberId == memberId && l.ReturnedDate == null);

            if (activeLoansCount >= 3)
                return (false, "You already have 3 active loans. Return a book first.", null);

            // All rules pass — create the loan
            var loan = new Loan
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowedDate = DateTime.UtcNow,
                ReturnedDate = null // Active loan
            };

            _db.Loans.Add(loan);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Loan created: Member {MemberId} borrowed Book {BookId}", memberId, bookId);

            return (true, "Book borrowed successfully.", loan);
        }

        public async Task<(bool Success, string Message)>
            ReturnBookAsync(int loanId, int memberId)
        {
            var loan = await _db.Loans.FindAsync(loanId);

            if (loan == null)
                return (false, "Loan not found.");

            // Security rule: A member can only return their OWNN loans
            if (loan.MemberId != memberId)
                return (false, "You are not authorized to return this loan.");

            if (loan.ReturnedDate != null)
                return (false, "This book has already been returned.");

            // Mark as returned with current timestamp
            loan.ReturnedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Loan {LoanId} returned by Member {MemberId}", loanId, memberId);

            return (true, "Book returned successfully.");
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansForMemberAsync(int memberId)
        {
            return await _db.Loans
                .Include(l => l.Book) // Load book info for display
                .Where(l => l.MemberId == memberId && l.ReturnedDate == null)
                .OrderByDescending(l => l.BorrowedDate)
                .ToListAsync();
        }
    }
}
