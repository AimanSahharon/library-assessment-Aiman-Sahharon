-- Library Assessment - Queries 


--  Q1: Top 5 most-borrowed books of all time 
-- Counts ALL loans (including returned ones) per book
SELECT TOP 5
    b.Title,
    b.Author,
    COUNT(l.Id) AS TotalTimesBorrowed
FROM dbo.Books b
LEFT JOIN dbo.Loans l ON l.BookId = b.Id
GROUP BY b.Id, b.Title, b.Author
ORDER BY TotalTimesBorrowed DESC;


-- Q2: Members with at least one overdue loan 
-- "Overdue" = active loan (ReturnedDate IS NULL) older than 14 days
SELECT
    m.FullName,
    m.Email,
    COUNT(l.Id) AS OverdueLoansCount
FROM dbo.Members m
INNER JOIN dbo.Loans l
    ON l.MemberId = m.Id
    AND l.ReturnedDate IS NULL                          -- Active only
    AND l.BorrowedDate < DATEADD(DAY, -14, GETUTCDATE()) -- Older than 14 days
GROUP BY m.Id, m.FullName, m.Email
HAVING COUNT(l.Id) >= 1
ORDER BY OverdueLoansCount DESC;


-- Q3: Loans per month for the last 12 months 
-- Uses a recursive CTE to generate all 12 months (including zero-loan months)
WITH MonthSeries AS (
    -- Anchor: first day of current month
    SELECT DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1) AS MonthStart
    UNION ALL
    -- Recurse back one month at a time, 12 iterations total
    SELECT DATEADD(MONTH, -1, MonthStart)
    FROM   MonthSeries
    WHERE  DATEADD(MONTH, -1, MonthStart) >= DATEADD(MONTH, -11,
               DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1))
)
SELECT
    FORMAT(ms.MonthStart, 'yyyy-MM')  AS YearMonth,
    COUNT(l.Id)                       AS TotalLoans
FROM MonthSeries ms
LEFT JOIN dbo.Loans l
    ON l.BorrowedDate >= ms.MonthStart
    AND l.BorrowedDate <  DATEADD(MONTH, 1, ms.MonthStart)
GROUP BY ms.MonthStart
ORDER BY ms.MonthStart;


-- Q4: Books that have never been borrowed
SELECT
    b.Id,
    b.Title,
    b.Author,
    b.ISBN
FROM dbo.Books b
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Loans l WHERE l.BookId = b.Id
);


-- Q5: Member with longest single loan duration 
-- Only considers RETURNED loans (both dates are not null)
SELECT TOP 1
    m.FullName  AS MemberName,
    b.Title     AS BookTitle,
    DATEDIFF(DAY, l.BorrowedDate, l.ReturnedDate) AS DaysBorrowed
FROM dbo.Loans l
INNER JOIN dbo.Members m ON m.Id = l.MemberId
INNER JOIN dbo.Books   b ON b.Id = l.BookId
WHERE l.ReturnedDate IS NOT NULL   -- Returned loans only
ORDER BY DaysBorrowed DESC;