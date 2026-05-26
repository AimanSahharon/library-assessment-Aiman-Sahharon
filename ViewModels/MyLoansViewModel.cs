using LibraryManagementSystemAimanSahharon.Models;

namespace LibraryManagementSystemAimanSahharon.ViewModels
{
    public class MyLoansViewModel
    {
        public IEnumerable<Loan> ActiveLoans { get; set; } = new List<Loan>();
        public IEnumerable<Loan> LoanHistory { get; set; } = new List<Loan>();
    }
}
