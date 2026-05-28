select * from Books

select * from Loans

select * from Members

Member

--Role: Member
--Librarian
UPDATE Members
SET Role = 'Librarian'
WHERE Email = 'aimansahharon44@gmail.com';

DELETE FROM Books;
DBCC CHECKIDENT ('Books', RESEED, 0);

DELETE FROM Loans;
DBCC CHECKIDENT ('Loans', RESEED, 0);

DELETE FROM Members;
DBCC CHECKIDENT ('Members', RESEED, 0);