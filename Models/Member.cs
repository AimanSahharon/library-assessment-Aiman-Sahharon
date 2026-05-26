using System.ComponentModel.DataAnnotations;


namespace LibraryManagementSystemAimanSahharon.Models
{
    public class Member
    {
        public int Id { get; set; }

        // Google's unique stable subject identifier ("sub" claim)
        // We use this to look up the member on subsequent logins
        [Required]
        [MaxLength(200)]
        public string SsoSubject { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        // Role: "Member" or "Librarian"
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Member";

        // Timestamp of first sign-in (auto-set on provisioning)
        public DateTime JoinedDate { get; set; }

        // Navigation property
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
