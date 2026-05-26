using LibraryManagementSystemAimanSahharon.Models;

namespace LibraryManagementSystemAimanSahharon.ViewModels
{
    public class BookAvailableViewModel
    {
        public IEnumerable<Book> Books { get; set; }
        public HashSet<int> BorrowedBookIds { get; set; }
    }
}
