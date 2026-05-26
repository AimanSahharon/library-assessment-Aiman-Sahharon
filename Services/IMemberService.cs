using LibraryManagementSystemAimanSahharon.Models;
using System.Security.Claims;

namespace LibraryManagementSystemAimanSahharon.Services
{
    public interface IMemberService
    {
        // Called on every sign-in: provisions new member or returns existing
        Task<Member> ProvisionOrGetMemberAsync(ClaimsPrincipal principal);

        // Looks up a member by their internal DB ID
        Task<Member?> GetMemberByIdAsync(int id);

        // Looks up by Google's sub claim
        Task<Member?> GetMemberBySsoSubjectAsync(string ssoSubject);
    }
}
