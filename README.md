# **Project Description:**


The Library Management System allows user to borrow up to 3 books and return the books. The librarian can add books and manage the book catalog. There are 2 user role one is Member that borrows the book and the other is Librarian


# **Prerequisite:**

1. **Visual Studio 2022**


    Download link: https://visualstudio.microsoft.com/downloads/


2. **SQL Server LocalDB (Already comes with Visual Studio 2022)**


3. **Google Cloud project with OAuth 2.0 credential (allows user to login with Google account)**


   Link: https://cloud.google.com/


4. **.NET 8 SDK**


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








   
