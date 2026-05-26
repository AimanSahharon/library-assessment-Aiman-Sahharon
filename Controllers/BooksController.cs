using LibraryManagementSystemAimanSahharon.Models;
using LibraryManagementSystemAimanSahharon.Services;
using LibraryManagementSystemAimanSahharon.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemAimanSahharon.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // GET /Books  — public, anyone can browse the catalog
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? author, string? title)
        {
            // Pass filter values back to the view to pre-fill the search boxes
            ViewBag.AuthorFilter = author;
            ViewBag.TitleFilter = title;

            var books = await _bookService.GetAllBooksAsync(author, title);
            return View(books);
        }

        // GET /Books/Details/5
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return View(book);
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
