-- Library Assessment - Schema

-- Drop tables in reverse dependency order (Loans first, then Books/Members)
IF OBJECT_ID('dbo.Loans', 'U') IS NOT NULL DROP TABLE dbo.Loans;
IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL DROP TABLE dbo.Books;
IF OBJECT_ID('dbo.Members', 'U') IS NOT NULL DROP TABLE dbo.Members;

-- BOOKS
CREATE TABLE dbo.Books (
    Id            INT           IDENTITY(1,1) NOT NULL,
    Title         NVARCHAR(300) NOT NULL,
    Author        NVARCHAR(200) NOT NULL,
    ISBN          NVARCHAR(20)  NOT NULL,
    PublishedYear INT           NOT NULL CHECK (PublishedYear BETWEEN 1000 AND 2100),
    TotalCopies   INT           NOT NULL CHECK (TotalCopies >= 1),

    CONSTRAINT PK_Books PRIMARY KEY (Id),
    -- Unique: no two rows can share the same ISBN
    CONSTRAINT UQ_Books_ISBN UNIQUE (ISBN)
);

-- Index on Author: filtering/searching by author is common
CREATE INDEX IX_Books_Author ON dbo.Books (Author);
-- Index on Title: supports partial title searches
CREATE INDEX IX_Books_Title  ON dbo.Books (Title);

-- MEMBERS
CREATE TABLE dbo.Members (
    Id          INT           IDENTITY(1,1) NOT NULL,
    SsoSubject  NVARCHAR(200) NOT NULL,   -- Google's stable "sub" claim
    FullName    NVARCHAR(200) NOT NULL,
    Email       NVARCHAR(200) NOT NULL,
    Role        NVARCHAR(50)  NOT NULL CONSTRAINT DF_Members_Role DEFAULT 'Member',
    JoinedDate  DATETIME2     NOT NULL,

    CONSTRAINT PK_Members   PRIMARY KEY (Id),
    CONSTRAINT UQ_Members_SsoSubject UNIQUE (SsoSubject),
    CONSTRAINT UQ_Members_Email      UNIQUE (Email),
    CONSTRAINT CK_Members_Role CHECK (Role IN ('Member', 'Librarian'))
);

-- LOANS 
CREATE TABLE dbo.Loans (
    Id           INT       IDENTITY(1,1) NOT NULL,
    BookId       INT       NOT NULL,
    MemberId     INT       NOT NULL,
    BorrowedDate DATETIME2 NOT NULL,
    ReturnedDate DATETIME2 NULL,  -- NULL = still on loan

    CONSTRAINT PK_Loans PRIMARY KEY (Id),

    CONSTRAINT FK_Loans_Books
        FOREIGN KEY (BookId) REFERENCES dbo.Books(Id) ON DELETE CASCADE,

    CONSTRAINT FK_Loans_Members
        FOREIGN KEY (MemberId) REFERENCES dbo.Members(Id) ON DELETE CASCADE
);

-- Composite index: most common query is "active loans for member X"
-- ReturnedDate IS NULL check benefits from this index
CREATE INDEX IX_Loans_Member_Returned ON dbo.Loans (MemberId, ReturnedDate);

-- Index for finding all loans of a book (e.g., counting available copies)
CREATE INDEX IX_Loans_BookId ON dbo.Loans (BookId);