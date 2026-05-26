namespace LibraryManagementSystemAimanSahharon.ViewModel
{
    // Returned by GET /me — shows the currently signed-in member's profile
    public class MemberProfileViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedDate { get; set; }
        public int ActiveLoansCount { get; set; }
    }
}
