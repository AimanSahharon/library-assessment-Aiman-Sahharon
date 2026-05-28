using LibraryManagementSystemAimanSahharon.Models;

namespace LibraryManagementSystemAimanSahharon.ViewModels
{
    public class BookAvailableDetailsViewModel
    {
        public Book Book { get; set; }
        public HashSet<int> BorrowedBookIds { get; set; }
    }
}
