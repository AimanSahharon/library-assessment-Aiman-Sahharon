using System.ComponentModel.DataAnnotations;


namespace LibraryManagementSystemAimanSahharon.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [Range(1000, 2100)]
        public int PublishedYear { get; set; }

        // Total physical copies owned by the library
        [Range(1, 1000)]
        public int TotalCopies { get; set; }

        // Navigation property — one book can have many loans
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

        // Computed: how many copies are NOT currently on loan
        // This is not stored in DB; calculated on the fly
        public int AvailableCopies =>
            TotalCopies - Loans.Count(l => l.ReturnedDate == null);
    }
}
