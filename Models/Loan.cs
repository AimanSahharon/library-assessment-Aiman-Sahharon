using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystemAimanSahharon.Models
{
    public class Loan
    {
        public int Id { get; set; }

        // Foreign key to Books table
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        // Foreign key to Members table
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        // When the member borrowed the book
        public DateTime BorrowedDate { get; set; }

        // Null means the book has NOT been returned yet (active loan)
        public DateTime? ReturnedDate { get; set; }

        // Helper property — true if the book hasn't been returned
        public bool IsActive => ReturnedDate == null;
    }
}
