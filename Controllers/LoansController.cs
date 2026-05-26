using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystemAimanSahharon.Services;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystemAimanSahharon.Controllers
{
    [Authorize] // All loan actions require sign-in
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoansController> _logger;

        public LoansController(ILoanService loanService, ILogger<LoansController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        // Helper: safely reads the memberId claim from the signed-in user's cookie
        private int? GetCurrentMemberId()
        {
            var claim = User.FindFirst("memberId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        // POST /Loans/Borrow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int bookId, string returnUrl)
        {
            var memberId = GetCurrentMemberId();
            if (memberId == null) return Unauthorized();

            var (success, message, _) =
                await _loanService.BorrowBookAsync(bookId, memberId.Value);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Books");


            // Redirect back to the book's detail page
            //return RedirectToAction("Details", "Books", new { id = bookId })
            return LocalRedirect(returnUrl);
        }

        // POST /Loans/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int loanId)
        {
            var memberId = GetCurrentMemberId();
            if (memberId == null) return Unauthorized();

            var (success, message) =
                await _loanService.ReturnBookAsync(loanId, memberId.Value);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message; // Includes 403-equivalent message for wrong member

            return RedirectToAction("MyLoans", "Members");
        }
    }
}
