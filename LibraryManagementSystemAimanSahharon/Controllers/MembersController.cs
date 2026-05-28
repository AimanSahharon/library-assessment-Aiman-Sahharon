using LibraryManagementSystemAimanSahharon.Services;
using LibraryManagementSystemAimanSahharon.ViewModel;
using LibraryManagementSystemAimanSahharon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemAimanSahharon.Controllers
{
    // All actions in this controller require sign-in
    [Authorize]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ILoanService _loanService;

        public MembersController(IMemberService memberService, ILoanService loanService)
        {
            _memberService = memberService;
            _loanService = loanService;
        }

        // GET /me — returns the current user's profile
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // Read memberId from the claim we stored in the cookie at sign-in
            var memberIdClaim = User.FindFirst("memberId")?.Value;

            // This should never be null for an authenticated user, but guard anyway
            if (memberIdClaim == null || !int.TryParse(memberIdClaim, out var memberId))
                return Unauthorized();

            var member = await _memberService.GetMemberByIdAsync(memberId);
            if (member == null) return NotFound();

            // Count active loans for display
            var activeLoans = await _loanService.GetActiveLoansForMemberAsync(memberId);

            var vm = new MemberProfileViewModel
            {
                Id = member.Id,
                FullName = member.FullName,
                Email = member.Email,
                Role = member.Role,
                JoinedDate = member.JoinedDate,
                ActiveLoansCount = activeLoans.Count()
            };

            return View(vm);
        }

        // GET /me/loans — shows active loans for the current member
        [HttpGet("me/loans")]
        public async Task<IActionResult> MyLoans()
        {
            var memberIdClaim = User.FindFirst("memberId")?.Value;
            if (memberIdClaim == null || !int.TryParse(memberIdClaim, out var memberId))
                return Unauthorized();

            var activeLoans = await _loanService.GetActiveLoansForMemberAsync(memberId);
            var historyLoans = await _loanService.GetLoanHistoryAsync(memberId);

            var vm = new MyLoansViewModel
            {
                ActiveLoans = activeLoans,
                LoanHistory = historyLoans
            };


            //var loans = await _loanService.GetActiveLoansForMemberAsync(memberId);

            return View(vm);
        }
    }
}
