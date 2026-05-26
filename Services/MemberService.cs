using LibraryManagementSystemAimanSahharon.Data;
using LibraryManagementSystemAimanSahharon.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystemAimanSahharon.Services
{
    public class MemberService : IMemberService
    {
        private readonly LibraryDbContext _db;
        private readonly ILogger<MemberService> _logger;

        public MemberService(LibraryDbContext db, ILogger<MemberService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Member> ProvisionOrGetMemberAsync(ClaimsPrincipal principal)
        {
            // Extract the Google "sub" claim — this is the stable unique identifier
            // ClaimTypes.NameIdentifier maps to the "sub" claim in OIDC tokens
            var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new InvalidOperationException("No sub claim found in token.");

            // Try to find an existing member with this sub
            var existing = await _db.Members
                .FirstOrDefaultAsync(m => m.SsoSubject == sub);

            if (existing != null)
            {
                _logger.LogInformation("Returning member signed in: {Email}", existing.Email);
                return existing; // Subsequent sign-in — return as-is
            }

            // First sign-in — read claims from the ID token to provision
            var email = principal.FindFirstValue(ClaimTypes.Email)
                        ?? throw new InvalidOperationException("No email claim.");

            // Google provides name in the "name" claim
            var fullName = principal.FindFirstValue("name")
                           ?? principal.FindFirstValue(ClaimTypes.Name)
                           ?? email; // Fallback to email if no name claim

            var newMember = new Member
            {
                SsoSubject = sub,
                Email = email,
                FullName = fullName,
                Role = "Member",             // All new sign-ins are Members by default
                JoinedDate = DateTime.Now // Provisioning timestamp
            };

            _db.Members.Add(newMember);
            await _db.SaveChangesAsync();

            _logger.LogInformation("New member provisioned: {Email} at {Time}",
                email, newMember.JoinedDate);

            return newMember;
        }

        public async Task<Member?> GetMemberByIdAsync(int id)
            => await _db.Members.FindAsync(id);

        public async Task<Member?> GetMemberBySsoSubjectAsync(string ssoSubject)
            => await _db.Members.FirstOrDefaultAsync(m => m.SsoSubject == ssoSubject);
    }
}
