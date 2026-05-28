-- Library Assessment - Seed Data

SET IDENTITY_INSERT dbo.Books ON;

INSERT INTO dbo.Books (Id, Title, Author, ISBN, PublishedYear, TotalCopies) VALUES
(1,  'Harry Potter and the Sorcerer''s Stone', 'J.K. Rowling','978-1338299144', 1997, 3),
(2,  'Project Hail Mary', 'Andy Weir', '978-0132350884', 2021, 4),
(3,  'The Da Vinci Code', 'Dan Brown', '978-0201633610', 2003, 2),
(4,  'The Hunger Games', 'Suzanne Collins', '978-0547928227', 2008, 5),
(5,  'Dune', 'Frank Herbert', '978-0441013593', 1965, 3),
(6,  'The Last Wish', 'Andrzej Sapkowski', '978-0735211292', 2007, 4),
(7,  'Skulduggery Pleasant', 'Derek Landy', '978-1455586691', 2016, 2),
(8,  'The Shining', 'Stephen King', '978-0307887894', 1977, 3),
(9,  'The Invisible Man', 'H. G. Wells',   '978-0062316097', 1897, 4),
(10, 'Marvel''s Spider-Man: Hostile Takeover', 'David Liss', '978-0262033848', 2018, 2),
(11, 'Starter Villain', 'John Scalzi', '978-1491924464', 2023, 3);

SET IDENTITY_INSERT dbo.Books OFF;

-- Members (SsoSubject is a fake Google sub for seed purposes)
SET IDENTITY_INSERT dbo.Members ON;

INSERT INTO dbo.Members (Id, SsoSubject, FullName, Email, Role, JoinedDate) VALUES
(1, 'google-sub-001', 'Alice Tan',     'alice@gmail.com',   'Librarian', '2020-01-10 08:00:00'),
(2, 'google-sub-002', 'Bob Lim',       'bob@gmail.com',     'Member',    '2024-02-15 09:30:00'),
(3, 'google-sub-003', 'Carol Wong',    'carol@gmail.com',   'Member',    '2021-03-01 10:00:00'),
(4, 'google-sub-004', 'David Ng',      'david@gmail.com',   'Member',    '2018-04-20 14:00:00'),
(5, 'google-sub-005', 'Eva Rashid',    'eva@gmail.com',     'Member',    '2021-01-05 11:00:00');

SET IDENTITY_INSERT dbo.Members OFF;

-- Loans: mix of returned and active
SET IDENTITY_INSERT dbo.Loans ON;

INSERT INTO dbo.Loans (Id, BookId, MemberId, BorrowedDate, ReturnedDate) VALUES
-- Returned loans
(1,  1,  2, '2025-06-01 10:00:00', '2026-06-10 15:00:00'),  -- Bob returned Harry Potter
(2,  2,  3, '2025-06-05 09:00:00', '2026-06-20 11:00:00'),  -- Carol returned Project Hail Mary
(3,  4,  4, '2025-06-10 08:00:00', '2026-06-25 10:00:00'),  -- David returned The Hunger Games
(4,  6,  5, '2026-07-01 09:00:00', '2026-07-15 14:00:00'),  -- Eva returned The Last Wish
(5,  9,  2, '2026-07-10 10:00:00', '2026-07-22 09:00:00'),  -- Bob returned The Invisible Man
(6,  1,  3, '2026-07-15 11:00:00', '2026-07-30 16:00:00'),  -- Carol returned Harry Potter
(7,  3,  4, '2026-08-01 09:00:00', '2026-08-12 14:00:00'),  -- David returned The Da Vinvi Code
-- Active loans (ReturnedDate IS NULL)
(8,  2,  2, DATEADD(DAY, -20, GETUTCDATE()), NULL),  -- Bob has Project Hail Mary (overdue)
(9,  5,  3, DATEADD(DAY, -5,  GETUTCDATE()), NULL),  -- Carol has Dune
(10, 7,  4, DATEADD(DAY, -16, GETUTCDATE()), NULL);  -- David has Skulduggery Pleasant (overdue)

SET IDENTITY_INSERT dbo.Loans OFF;