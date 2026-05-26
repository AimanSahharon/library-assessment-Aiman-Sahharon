using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystemAimanSahharon.ViewModel
{
    // Used for Create and Edit forms to separates form data from the DB model
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(300)]
        [Display(Name = "Book Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        [MaxLength(200)]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required.")]
        [MaxLength(20)]
        [RegularExpression(@"^[\d\-X]+$", ErrorMessage = "ISBN must contain digits, hyphens, or X.")]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        [Range(1000, 2100, ErrorMessage = "Enter a valid publication year.")]
        [Display(Name = "Published Year")]
        public int PublishedYear { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Must have at least 1 copy.")]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; }
    }
}
