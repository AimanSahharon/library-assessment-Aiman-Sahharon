using LibraryManagementSystemAimanSahharon.Models;


namespace LibraryManagementSystemAimanSahharon.Services
{
    public interface ILoanService
    {
        Task<(bool Success, string Message, Loan? Loan)> BorrowBookAsync(int bookId, int memberId);
        Task<(bool Success, string Message)> ReturnBookAsync(int loanId, int memberId);
        Task<IEnumerable<Loan>> GetActiveLoansForMemberAsync(int memberId);
        Task<List<Loan>> GetLoanHistoryAsync(int memberId);
    }
}
