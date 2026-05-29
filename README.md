# **Project Description:**


The Library Management System allows user to borrow up to 3 books and return the books. The librarian can add books and manage the book catalog. There are 2 user role one is Member and the other is Librarian


# **Prerequisite:**

1. **Visual Studio 2022**


    Download link: https://visualstudio.microsoft.com/downloads/


2. **SQL Server LocalDB (Already comes with Visual Studio 2022)**


3. **Google Cloud project with OAuth 2.0 credential (allows user to login with Google account)**


   Link: https://cloud.google.com/


4. **.NET 8 SDK**


5. **SQL Server Management Studio 21 (same function as SQL Server LocalDB in Visual Studio but easier to manage data)**


   Download link: https://learn.microsoft.com/en-us/ssms/install/install


**Dependencies:**


- Microsoft.EntityFrameworkCore.Tools version 8.0.27

  
- Microsoft.EntityFrameworkCore.SqlServer version 8.0.27

  
- Microsoft.EntityFrameworkCore.Design version 8.0.27

  
- Microsoft.AspNetCore.Authentication.JwtBearer version 8.0.27

  
- FluentValidation.AspNetCore version 11.3.1

  
- moq version 4.20.72

  
- fluentAssertions version 8.10.0
  

- Microsoft.AspNetCore.Authentication.Google version 8.0.27


- Microsoft.EntityFrameworkCore.InMemory version 8.0.27


This project uses Entity Framework Core for the following reasons:

1. **Migration tooling** — EF Core generates and applies schema migrations
   automatically via `Add-Migration` and `Update-Database`. This keeps the
   database schema always in sync with the C# model classes without
   maintaining separate DDL scripts.

2. **Fits the domain** — the Library domain has three related tables with
   foreign keys and navigation properties. EF Core's `Include()` handles
   JOIN loading in one line, and LINQ queries are strongly typed so
   mistakes are caught at compile time rather than at runtime.

3. **Single source of truth** — DataAnnotations on the model classes drive
   both the database schema (column lengths, unique indexes, constraints)
   and ASP.NET Core's request validation simultaneously. There is no risk
   of the two getting out of sync.

4. **Appropriate for this workload** — a library management system has low
   concurrency and straightforward queries. EF Core's change tracking and
   LINQ translation are well suited to this type of domain without
   introducing unnecessary complexity.


# **Setup:**

**1. Clone the Repository**

Go to https://github.com/AimanSahharon/library-assessment-Aiman-Sahharon and click on the green "Code" button to copy the repository link


Repository Link:

```bash
https://github.com/AimanSahharon/library-assessment-Aiman-Sahharon.git
```

<img width="630" height="590" alt="Screenshot 2026-05-28 204721" src="https://github.com/user-attachments/assets/9c30dfdb-deea-4af8-85c1-481127834b88" />


Open Visual Studio 2022 app and click on "Clone a repository" button


<img width="611" height="601" alt="Screenshot 2026-05-28 210255" src="https://github.com/user-attachments/assets/bf0e68b2-ab36-45ae-81a3-c155bef3fe9c" />


Paste the repository link under "Repository Location" textbok and click on "Clone" button on the bottom left


<img width="2558" height="1362" alt="Screenshot 2026-05-28 210518" src="https://github.com/user-attachments/assets/e6584b32-bf1e-41b5-831e-89bd5cdafc0d" />


Right click on Solution 'LibraryManagementSystemAimanSahharon' and click on "Manage NuGet Packages for Solution..." 


<img width="608" height="441" alt="Screenshot 2026-05-28 211542" src="https://github.com/user-attachments/assets/b62688ff-c989-4e03-aeb4-f801c6f25438" />

**2. Download Dependencies**


Click on "Browse" tab and use the search bar to install dependencies


<img width="998" height="821" alt="Screenshot 2026-05-28 211911" src="https://github.com/user-attachments/assets/a4838c68-7e12-4ea7-988e-e1f7db87b652" />


(**Important Note:** because we are using .NET 8 please make sure the version of the dependencies are the latest 8.x.xx version)


**Download the following dependencies:**

- Microsoft.EntityFrameworkCore.Tools version 8.0.27

  
- Microsoft.EntityFrameworkCore.SqlServer version 8.0.27

  
- Microsoft.EntityFrameworkCore.Design version 8.0.27

  
- Microsoft.AspNetCore.Authentication.JwtBearer version 8.0.27

  
- FluentValidation.AspNetCore version 11.3.1

  
- moq version 4.20.72

  
- fluentAssertions version 8.10.0
  

- Microsoft.AspNetCore.Authentication.Google version 8.0.27


**3. Setup Google Cloud OAuth**

Setting up Google cloud allows user to sign in with their Google account.


Go to https://cloud.google.com/ and click on the "Console" link and create and setup the project and obtain the Client ID and Client Secret


under "Authorized redirect URIs"


Add the following URL:


```bash
https://localhost:7238
```


```bash
https://localhost:7238/signin-google
```


```bash
https://localhost:7238/signin-oidc
```


Go back to Visual Studio 2022 and find appsetting.json file then find this section of the code and insert your clientId and ClientSecret (replace "your-google-client-id" and "your-google-client-secret" with actual clientId and ClientSecret)

```bash
"Authentication": {
  "Google": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret"
  }
```

# **Setting up Database:**


In Visual Studio 2022 got to View > Terminal and select Developer Powershell


<img width="1497" height="330" alt="Screenshot 2026-05-28 231440" src="https://github.com/user-attachments/assets/1056c3ad-8e55-48f8-9da3-42f6d2b82a16" />

Type the following command to read the migration files and set up the database

```bash
dotnet tool install --global dotnet-ef
```


```bash
dotnet restore
```


```bash
dotnet ef database update
```


Then click on View > SQL Server Object Explorer


Then click on (loacldb)\\MSSQLLocalDB > Databases then LibraryAssessmentDb should appear based on this snipet of code in appsettings.json


```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;**Database=LibraryAssessmentDb**;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
},
```


# **Seeding data into database:**


In Visual Studio 2022

1. Open View > SQL Server Object Explorer


2. Find your database e.g "LibraryAssessmentDb"


3. Right-click database > New Query


4. Open seed.sql and copy all content


5. Paste script > Click Execute


<img width="1481" height="792" alt="Screenshot 2026-05-28 233153" src="https://github.com/user-attachments/assets/487bf768-d58d-4444-83b7-0723bb8378b8" />


# **Running Application**


Click on "https" green button to start the application


<img width="512" height="168" alt="Screenshot 2026-05-28 233913" src="https://github.com/user-attachments/assets/47e4ee73-474d-48e9-8785-93398e4c6dd1" />


user will be greeted with the home page and click "Login with Google" and select the Google account to sign in 


<img width="2559" height="1365" alt="Screenshot 2026-05-28 233759" src="https://github.com/user-attachments/assets/160a2f4e-f58d-4524-bcf9-09182c6e4b26" />


Once login, the button will change allowing user to straight away go to Book page to start borrow their book


<img width="2559" height="1368" alt="Screenshot 2026-05-28 233813" src="https://github.com/user-attachments/assets/0dac49f2-eb18-4c1a-af88-f408a46ee282" />


The Book page allows Members to borrow the books as well as search title of the book, author, ISBN or sort by alphabetical order. Once they borrow a book the "Borrow" button will gray out assuming Member are only allowed to borrow 1 copy of the book. The button will also gray out if the copies of the book are unavailable. 


<img width="2559" height="1371" alt="Screenshot 2026-05-28 234357" src="https://github.com/user-attachments/assets/1fc7de33-7007-4ddf-8be0-bb79dfbee36f" />


My Profile page shows when the Member joined, what their role are and how many books they can borrow before reaching the limit



<img width="2544" height="1364" alt="Screenshot 2026-05-28 234403" src="https://github.com/user-attachments/assets/34d17f6f-5c5d-412a-8c3c-fc9bc56fed42" />


At My Loans page is where Member can view their active book currently borrowing and history of books they have borrowed. 


<img width="2559" height="1363" alt="Screenshot 2026-05-28 234840" src="https://github.com/user-attachments/assets/f3e8bac7-7861-405c-9fec-8662026c4dd9" />


# **Assigning Roles**


By default newly login user will be assign as Member role


To assign user as Librarian once login run the following query either in Visual Studio 2022 or SQL Server Management Studio 21 

```bash
UPDATE Members
SET Role = 'Librarian'
WHERE Email = 'user@gmail.com';
```


Only the Librarian role can do CRUD functions to create new books, edit, update and delete. The Add New Book page allows Librarian to add new book to the catalog. 


<img width="2559" height="1367" alt="Screenshot 2026-05-28 233829" src="https://github.com/user-attachments/assets/4f8c5b26-6602-4a79-8185-74e0d389d8cb" />


At the My Book page, Librarian can still borrow books and has an addition option to edit and delete 


<img width="2559" height="1362" alt="Screenshot 2026-05-28 233837" src="https://github.com/user-attachments/assets/fba1710b-4579-4416-b05d-28d2d230607c" />


At My Profile page it shows the Librarian role has been assign


<img width="2559" height="1366" alt="Screenshot 2026-05-28 233844" src="https://github.com/user-attachments/assets/88372f78-4c53-42e5-8fd5-71eda278c373" />


# **Running the Test**

In Visual Studio 2022 go to View > Terminal


run the following command:
```bash]
dotnet test
```


This will run LoanServiceTest.cs under LibraryManagementSystemAimanSahharon.Test using xUnit Test Project
<img width="924" height="326" alt="Screenshot 2026-05-29 000035" src="https://github.com/user-attachments/assets/e85992e2-9a5c-4bb2-b23d-545d3206ee7e" />


# **Using SQL Server Management Studio 21**

Once launch, click on "Connect Object Explorer" button next to the Connect dropdown in Object Explorer window. 


type the following to connect to localdb:


```bash
(localdb)\MSSQLLocalDB
```


<img width="711" height="792" alt="Screenshot 2026-05-29 082719" src="https://github.com/user-attachments/assets/16a72f21-4fc3-4f29-99f3-d9d8074cd1d2" />


Right click on the server and click on "New Query" and paste the schema.sql to create the tables and seed.sql for sample data. 








