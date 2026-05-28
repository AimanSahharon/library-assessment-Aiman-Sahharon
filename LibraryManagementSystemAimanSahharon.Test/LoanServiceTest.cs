using LibraryManagementSystemAimanSahharon.Data;
using LibraryManagementSystemAimanSahharon.Models;
using LibraryManagementSystemAimanSahharon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace LibraryManagementSystemAimanSahharon.Test
{
    public class LoanServiceTests
    {
        // Creates a fresh in-memory database for each test (no SQL Server needed)
        private LibraryDbContext CreateDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new LibraryDbContext(options);
        }

        // ?? Test 1: Borrowing succeeds when copies are available ?????????????
        [Fact]
        public async Task BorrowBook_WhenCopiesAvailable_ReturnsSuccess()
        {
            using var db = CreateDb("test_borrow_success");

            // Arrange: add a book with 2 copies and a member with no loans
            db.Books.Add(new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Author",
                ISBN = "111",
                PublishedYear = 2020,
                TotalCopies = 2
            });
            db.Members.Add(new Member
            {
                Id = 1,
                SsoSubject = "sub1",
                FullName = "Alice",
                Email = "alice@test.com",
                Role = "Member",
                JoinedDate = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var service = new LoanService(db, NullLogger<LoanService>.Instance);

            // Act
            var (success, message, loan) = await service.BorrowBookAsync(1, 1);

            // Assert
            Assert.True(success);
            Assert.NotNull(loan);
            Assert.Equal("Book borrowed successfully.", message);
        }

        // ?? Test 2: Borrowing is rejected when no copies available ???????????
        [Fact]
        public async Task BorrowBook_WhenNoCopiesAvailable_ReturnsFailure()
        {
            using var db = CreateDb("test_no_copies");

            // Only 1 copy, already on loan
            var book = new Book
            {
                Id = 1,
                Title = "Busy Book",
                Author = "Author",
                ISBN = "222",
                PublishedYear = 2020,
                TotalCopies = 1
            };
            db.Books.Add(book);
            db.Members.Add(new Member
            {
                Id = 1,
                SsoSubject = "sub1",
                FullName = "Bob",
                Email = "bob@test.com",
                Role = "Member",
                JoinedDate = DateTime.UtcNow
            });
            db.Members.Add(new Member
            {
                Id = 2,
                SsoSubject = "sub2",
                FullName = "Carol",
                Email = "carol@test.com",
                Role = "Member",
                JoinedDate = DateTime.UtcNow
            });
            // Member 1 already has the only copy
            db.Loans.Add(new Loan
            {
                BookId = 1,
                MemberId = 1,
                BorrowedDate = DateTime.UtcNow,
                ReturnedDate = null
            });
            await db.SaveChangesAsync();

            var service = new LoanService(db, NullLogger<LoanService>.Instance);

            // Act: Member 2 tries to borrow
            var (success, message, loan) = await service.BorrowBookAsync(1, 2);

            // Assert
            Assert.False(success);
            Assert.Null(loan);
            Assert.Contains("No copies available", message);
        }

        // ?? Test 3: Borrowing is rejected when member has 3 active loans ?????
        [Fact]
        public async Task BorrowBook_WhenMemberHas3ActiveLoans_ReturnsFailure()
        {
            using var db = CreateDb("test_loan_limit");

            for (int i = 1; i <= 4; i++)
                db.Books.Add(new Book
                {
                    Id = i,
                    Title = $"Book {i}",
                    Author = "Author",
                    ISBN = $"ISBN-{i}",
                    PublishedYear = 2020,
                    TotalCopies = 5
                });

            db.Members.Add(new Member
            {
                Id = 1,
                SsoSubject = "sub1",
                FullName = "Dave",
                Email = "dave@test.com",
                Role = "Member",
                JoinedDate = DateTime.UtcNow
            });

            // Add 3 existing active loans for member 1
            for (int i = 1; i <= 3; i++)
                db.Loans.Add(new Loan
                {
                    BookId = i,
                    MemberId = 1,
                    BorrowedDate = DateTime.UtcNow,
                    ReturnedDate = null
                });

            await db.SaveChangesAsync();

            var service = new LoanService(db, NullLogger<LoanService>.Instance);

            // Act: try to borrow a 4th book
            var (success, message, loan) = await service.BorrowBookAsync(4, 1);

            // Assert
            Assert.False(success);
            Assert.Contains("3 active loans", message);
        }

        // ?? Test 4: Return is rejected if the loan belongs to another member ??
        [Fact]
        public async Task ReturnBook_WhenLoanBelongsToOtherMember_ReturnsFailure()
        {
            using var db = CreateDb("test_return_wrong_member");

            db.Books.Add(new Book
            {
                Id = 1,
                Title = "Book",
                Author = "Author",
                ISBN = "333",
                PublishedYear = 2020,
                TotalCopies = 2
            });
            db.Members.Add(new Member
            {
                Id = 1,
                SsoSubject = "sub1",
                FullName = "Eve",
                Email = "eve@test.com",
                Role = "Member",
                JoinedDate = DateTime.UtcNow
            });
            db.Members.Add(new Member
            {
                Id = 2,
                SsoSubject = "sub2",
                FullName = "Frank",
                Email = "frank@test.com",
                Role = "Member",
                JoinedDate = DateTime.UtcNow
            });
            // Loan belongs to member 1
            db.Loans.Add(new Loan
            {
                Id = 1,
                BookId = 1,
                MemberId = 1,
                BorrowedDate = DateTime.UtcNow,
                ReturnedDate = null
            });
            await db.SaveChangesAsync();

            var service = new LoanService(db, NullLogger<LoanService>.Instance);

            // Act: member 2 tries to return member 1's loan
            var (success, message) = await service.ReturnBookAsync(1, memberId: 2);

            Assert.False(success);
            Assert.Contains("not authorized", message);
        }

        // ?? Test 5: SSO provisioning creates a new member on first sign-in ???
        [Fact]
        public async Task ProvisionOrGetMember_NewUser_CreatesMemberRecord()
        {
            using var db = CreateDb("test_provision");

            // Simulate a ClaimsPrincipal with Google claims
            var claims = new[]
            {
                new System.Security.Claims.Claim(
                    System.Security.Claims.ClaimTypes.NameIdentifier, "google-sub-xyz"),
                new System.Security.Claims.Claim(
                    System.Security.Claims.ClaimTypes.Email, "newuser@gmail.com"),
                new System.Security.Claims.Claim("name", "New User")
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "Google");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            var service = new MemberService(db, NullLogger<MemberService>.Instance);

            // Act
            var member = await service.ProvisionOrGetMemberAsync(principal);

            // Assert
            Assert.NotNull(member);
            Assert.Equal("google-sub-xyz", member.SsoSubject);
            Assert.Equal("newuser@gmail.com", member.Email);
            Assert.Equal("New User", member.FullName);
            Assert.Equal("Member", member.Role);
            Assert.Equal(1, await db.Members.CountAsync()); // Only one record created
        }
    }
}