## Creating a new project using Visual Studio

1. Create and add a new project named BackEnd and name the solution ConferencePlanner using File / New / ASP.NET Core Web Application. Select the Web API template, No Auth, no Docker support.

![alt text](img/vs2019-new-project.png)
![alt text](img/image.png)
![alt text](img/Third.png)

> **Note:** If not using Visual Studio, create the project using `dotnet new webapi` at the cmd line, details as follows:
>
> i. Create folder ConferencePlanner and call `dotnet new sln` at the cmd line to create a solution
>
> ii. Create sub-folder BackEnd and create a project using `dotnet new webapi` at the cmd line inside the folder BackEnd
>
> iii. From the ConferencePlanner folder, add the project to the solution using `dotnet sln add BackEnd/BackEnd.csproj`

2. Add a new `Models` folder to the BackEnd project.

3. Add a new `Speaker` class inside the Models folder using the following code:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Speaker
    {
       public int Id { get; set; }

       [Required]
       [StringLength(200)]
       public string Name { get; set; }

       [StringLength(4000)]
       public string Bio { get; set; }

       [StringLength(1000)]
       public virtual string WebSite { get; set; }
    }
}
```

4. Add a reference in the BackEnd project to the NuGet package `Microsoft.EntityFrameworkCore.SqlServer` version `9.0.1.`

> This can be done from the command line in the `BackEnd` folder using `dotnet add package Microsoft.EntityFrameworkCore.SqlServer` --version `9.0.1`

5. Add a reference in the BackEnd project to the NuGet package `Npgsql.EntityFrameworkCore.PostgreSQL` --version `9.0.3`

> This can be done from the command line in the `BackEnd` folder using `dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL` --version `9.0.3`

6. Next we'll create a new Entity Framework DbContext. Create a new `ApplicationDbContext` class in the `Models` folder using the following code:

```csharp
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Speaker> Speakers { get; set; }
    }
}
```
7. Add a connection string to the `appsettings.json` file for this database:

```csharp
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=YourDatabaseName;Username=YourUsername;Password=YourPassword"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```
## Register the DB Context Service

1. Add the following code to the top of the `builder.Build();` method in `Program.cs`:

```csharp
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
```
> This code registers the `ApplicationDbContext` service so it can be injected into controllers. Additionally, it configures operating system specific database technologies and connection strings

2. Ensure the following `using` statements are at the top of the Program.cs file to allow the references in our code to work

```csharp
using BackEnd.Models;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
```
## Configuring EF Migrations

1. Add a reference to the NuGet package `Microsoft.EntityFrameworkCore.Tools` --version `9.0.1`.
> If you're not using Visual Studio install the package from the command line in the `BackEnd` folder with `dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.1`

**Visual Studio: Package Manager Console**

1. In Visual Studio, select the Tools -> NuGet Package Manager -> Package Manager Console

2. Run the following commands in the Package Manager Console

```csharp
Add-Migration Initial
Update-Database
```
**Command line**

1. Install the EntityFramework global tool `dotnet-ef` using the following command in the `BackEnd` folder:

```markdown
dotnet tool install -g dotnet-ef --version 3.1.3
```
2. Open a command prompt and navigate to the project directory. (The directory containing the `Program.cs` file).

3. Run the following commands in the command prompt:

```markdown
dotnet build
dotnet ef migrations add Initial
dotnet ef database update
```
## Commands Explained
| **Command** | **Description** |
|-------------|-----------------|
| `dotnet ef migrations add Initial` / `Add-Migration Initial` | generates code to create the initial database schema based on the model specified in `ApplicationDbContext.cs`. `Initial` is the name of the migration. |
| `dotnet ef database update` / `Update-Database` | creates the database |

> For more information on these commands and scaffolding in general, see ![this tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-model)
