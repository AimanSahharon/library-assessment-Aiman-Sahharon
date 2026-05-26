using LibraryManagementSystemAimanSahharon.Models;
using LibraryManagementSystemAimanSahharon.Services;
using LibraryManagementSystemAimanSahharon.ViewModel;
using LibraryManagementSystemAimanSahharon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LibraryManagementSystemAimanSahharon.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;
        private readonly ILoanService _loanService;

        public BooksController(IBookService bookService, ILogger<BooksController> logger, ILoanService loanService)
        {
            _bookService = bookService;
            _logger = logger;
            _loanService = loanService;
        }

        private int? GetCurrentMemberId()
        {
            var claim = User.FindFirst("memberId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        // GET /Books  — public, anyone can browse the catalog
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? author, string? title, string? sortBy, string? isbn)
        {
            // Pass filter values back to the view to pre-fill the search boxes
            ViewBag.AuthorFilter = author;
            ViewBag.TitleFilter = title;
            ViewBag.SortBy = sortBy;
            ViewBag.SortBy = isbn;

            var books = await _bookService.GetAllBooksAsync(author, title, sortBy, isbn);

            HashSet<int> borrowedBookIds = new();

            var memberId = GetCurrentMemberId(); // add this method in BooksController

            if (memberId != null)
            {
                borrowedBookIds = await _loanService.GetActiveLoansForMemberAsync(memberId.Value)
                    .ContinueWith(t =>
                        t.Result.Select(l => l.BookId).ToHashSet()
                    );
            }

            var vm = new BookAvailableViewModel
            {
                Books = books,
                BorrowedBookIds = borrowedBookIds
            };

            return View(vm);


            return View(books);
        }

        // GET /Books/Details/5
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            var memberId = GetCurrentMemberId();

            HashSet<int> borrowedBookIds = new();

            if (memberId != null)
            {
                borrowedBookIds = (await _loanService.GetActiveLoansForMemberAsync(memberId.Value))
                    .Select(l => l.BookId)
                    .ToHashSet();
            }

            var vm = new BookAvailableDetailsViewModel
            {
                Book = book,
                BorrowedBookIds = borrowedBookIds
            };

            return View(vm);
            //return View(book);
        }

        // GET /Books/Create — only Librarians can see this form
        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public IActionResult Create()
        {
            return View(new BookViewModel());
        }

        // POST /Books/Create
        [HttpPost]
        [Authorize(Roles = "Librarian")]
        [ValidateAntiForgeryToken] // Prevents cross-site request forgery
        public async Task<IActionResult> Create(BookViewModel vm)
        {
            // ModelState.IsValid checks DataAnnotations on BookViewModel
            if (!ModelState.IsValid)
                return View(vm); // Return form with validation errors

            if (await _bookService.IsIsbnExistsAsync(vm.ISBN))
            {
                ModelState.AddModelError("ISBN", "ISBN number already exists.");
                return View(vm);
            }

            if (vm.PublishedYear > DateTime.Now.Year)
            {
                ModelState.AddModelError("PublishedYear", "Published year cannot be in the future.");
                return View(vm);
            }

            var book = new Book
            {
                Title = vm.Title,
                Author = vm.Author,
                ISBN = vm.ISBN,
                PublishedYear = vm.PublishedYear,
                TotalCopies = vm.TotalCopies
            };

            await _bookService.CreateBookAsync(book);
            TempData["Success"] = $"'{book.Title}' added to the catalog.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Books/Edit/5
        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            // Map DB model to ViewModel for the form
            var vm = new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                TotalCopies = book.TotalCopies
            };
            return View(vm);
        }

        // POST /Books/Edit/5
        [HttpPost]
        [Authorize(Roles = "Librarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Check if book exists
            var existingBook = await _bookService.GetBookByIdAsync(id);
            if (existingBook == null)
                return NotFound();

            // Check ISBN duplicate (IMPORTANT: exclude current book)
            var isDuplicate = await _bookService.IsIsbnExistsAsync(vm.ISBN);

            if (isDuplicate && vm.ISBN != existingBook.ISBN)
            {
                ModelState.AddModelError("ISBN", "ISBN number already exists.");
                return View(vm);
            }

            // Validate year
            if (vm.PublishedYear > DateTime.Now.Year)
            {
                ModelState.AddModelError("PublishedYear", "Published year cannot be in the future.");
                return View(vm);
            }


            var updated = new Book
            {
                Title = vm.Title,
                Author = vm.Author,
                ISBN = vm.ISBN,
                PublishedYear = vm.PublishedYear,
                TotalCopies = vm.TotalCopies
            };

            var result = await _bookService.UpdateBookAsync(id, updated);
            if (result == null) return NotFound();

            TempData["Success"] = "Book updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Books/Delete/5
        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return View(book); // Confirmation page
        }

        // POST /Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Librarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _bookService.DeleteBookAsync(id);
            if (!deleted) return NotFound();

            TempData["Success"] = "Book removed from catalog.";
            return RedirectToAction(nameof(Index));
        }
    }
}
