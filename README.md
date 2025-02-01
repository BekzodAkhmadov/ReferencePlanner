## Creating a new project using Visual Studio

1. Create and add a new project named BackEnd and name the solution ConferencePlanner using File / New / ASP.NET Core Web Application. Select the Web API template, No Auth, no Docker support.

![alt text](img/vs2019-new-project.png)
![alt text](img/image.png)
![alt text](img/image-1.png)

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

> For more information on these commands and scaffolding in general, see [this tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-model)

> If your database ever gets in a bad state and you'd like to reset things, you can use `dotnet ef database drop` followed by `dotnet ef database update` to remove your database and run all migrations again.

## A quick look at the Weather Forecast Controller

First, open the `Controllers` folder and take a quick look at the `WeatherForecastController`. You'll see a simple function that corresponds to the HTTP GET verb. You'll see the output of this controller in a bit, but first we'll build our own API controller for the `Speakers` model class.

## Scaffolding an API Controller

**Using Visual Studio**

1. Right-click the `Controllers` folder and select Add/Controller. Select "API Controller with actions, using Entity Framework".

2. In the dialog, select the `Speaker` model for the Model Class, `ApplicationDbContext` for the "Data Context Class" and click the Add button.

![alt text](img/Fourth.png)

**Using the cmd line**

1. Install the "Microsoft.VisualStudio.Web.CodeGeneration.Design" package

```markdown
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 9.0.0
```
2. Install the `aspnet-codegenerator` global tool by running the following command:

```markdown
dotnet tool install --global dotnet-aspnet-codegenerator --version 9.0.0
```
> Note: You will need to close and reopen the console window to be able to use this tool.

3. Run the following in the project folder at the cmd line

```markdown
dotnet aspnet-codegenerator controller -api -name SpeakersController -m Speaker -dc BackEnd.Models.ApplicationDbContext -outDir Controllers
```
## Testing the API using the Swashbuckle

1. Add a reference to the NuGet package `Swashbuckle.AspNetCore` --version `7.2.0`

> This can be done from the command line using `dotnet add package Swashbuckle.AspNetCore --version 7.2.0`

2. Add the Swashbuckle services in your Program.cs:

```csharp
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Conference Planner API", 
        Version = "v1" 
    })
);
```
3. Ensure your `Program.cs` file contains the following 'using' statements:

```csharp
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;
```
4. Configure Swashbuckle by adding the following lines just before `UseRouting` in the `Configure` method in `Program.cs`:

```markdown
app.UseSwagger();

app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Planner API v1")
);
```
5. Add a MapGet to the beginning of the `app.MapGet()` statement in the pipeline that redirects requests from the root of our application to the swagger end point.

```csharp
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/");
    return Task.CompletedTask;
});
```
6. Run the application (F5 in Visual Studio or `dotnet run` from console).

## Building out the Back End

In this session, we'll add the rest of our models and controllers that expose them. We'll also refactor our application, moving our DTOs to a shared project so they can be used by our front-end application later.

## Add a ConferenceDTO project

> We'll start by creating the new shared project to hold our data transfer objects. 

**Adding the ConferenceDTO Project using Visual Studio**

1. If using Visual Studio, right-click on the Solution and select *Add / New Project....*
2. Select *.NET Standard* from the project types on the left and select the *Class Library (.NET Standard)* template. Name the project ConferenceDTO and press OK.
![alt text](img/Fifth.png)
3. Delete the generated `Class1.cs` file from this new project.
4. Right-click the 'Dependencies' node under the BackENd project, select "Add Reference..." and put a checkmark near ConferenceDTO.

**Adding the ConferenceDTO project via the Command Line**

1. Open a command prompt and navigate to the root `ConferencePlanner` directory.
2. Run the following command:

```markdown
dotnet new classlib -o ConferenceDTO -f net6.0
```
3. Next we'll need to add a reference to the ConferenceDTO project from the BackEnd project. From the command line, navigate to the BackEnd project directory and execute the following command:

```markdown
dotnet add reference ../ConferenceDTO
```
4. Add the ConferenceDTO project to the solution:

```markdown
dotnet sln add ConferenceDTO/ConferenceDTO.csproj
```
**Refactoring the Speaker model into the ConferenceDTO project**

1. Copy the `Speaker.cs` class from the *BackEnd* application into the root of the new ConferenceDTO project, and change the namespace to `ConferenceDTO`
2. The data annotations references should be broken at this point, to resovle it, we need to add a nuget the missing NuGet package into the `ConferenceDTO` project.
3. Add a reference to the NuGet package `System.ComponentModel.Annotations` --version `7.0.0`
> This can be done from the command line using `dotnet add package System.ComponentModel.Annotations --version 7.0.0` 
4. When the package restore completes, you should see that your data annotations are now resolved.
5. Go back to the *BackEnd* application and modify the code in `Speaker.cs`as shown:

```csharp
public class Speaker : ConferenceDTO.Speaker
{
}
```
6. Run the application and view the Speakers data using the Swagger UI to verify everything still works.

## Adding the remaining models to ConferenceDTO

We've got several more models to add, and unfortunately it's a little mechanical. You can copy the following classes manually, or open the completed solution which is shown at the end.

1. Create an `Attendee.cs` class in the ConferenceDTO project with the following code:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConferenceDTO
{
    public class Attendee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public virtual string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public virtual string LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string UserName { get; set; }
        
        [StringLength(256)]
        public virtual string EmailAddress { get; set; }
    }
}
```
2. Create a `Session.cs` class with the following code

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConferenceDTO
{
    public class Session
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(4000)]
        public virtual string Abstract { get; set; }

        public virtual DateTimeOffset? StartTime { get; set; }

        public virtual DateTimeOffset? EndTime { get; set; }

        // Bonus points to those who can figure out why this is written this way
        public TimeSpan Duration => EndTime?.Subtract(StartTime ?? EndTime ?? DateTimeOffset.MinValue) ?? TimeSpan.Zero;

        public int? TrackId { get; set; }
    }
}
```
3. Create a new `Track.cs` class with the following code:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConferenceDTO
{
    public class Track
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }
    }
}
```
## Creating Derived Models in the BackEnd project

We're not going to create our EF models directly from the `ConferenceDTO` classes. Instead, we'll create some composite classes such as `SessionSpeaker`, since these will map more closely to what our application will be working with.

We're also going to take this opportunity to rename the `Models` directory in the *BackEnd* project to `Data` since it no longer just contains models.

1. Right-click the `Models` directory and select `Rename`, changing the name to `Data`.
> Note: If you are using Visual Studio, you can use refactoring to rename the namespace.
2. Add a `SessionSpeaker.cs` class to the `Data` directory with the following code:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Data
{
    public class SessionSpeaker
    {
        public int SessionId { get; set; }

        public Session Session { get; set; }

        public int SpeakerId { get; set; }

        public Speaker Speaker { get; set; }
    }
}
```
3. Add an `SessionAttendee.cs` class with the following code:

```csharp
using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Threading.Tasks;

 namespace BackEnd.Data
 {
     public class SessionAttendee
     {
         public int SessionId { get; set; }

         public Session Session { get; set; }

         public int AttendeeId { get; set; }

         public Attendee Attendee { get; set; }
    }
}
```
4. Add an `Attendee.cs` class with the following code:

```csharp
using System;
using System.Collections.Generic;

namespace BackEnd.Data
{
    public class Attendee : ConferenceDTO.Attendee
    {
        public virtual ICollection<SessionAttendee> SessionsAttendees { get; set; }
    }
}
```
5. Add a `Session.cs` class with the following code:

```csharp
using System;
using System.Collections;
using System.Collections.Generic;

namespace BackEnd.Data
{
    public class Session : ConferenceDTO.Session
    {
        public virtual ICollection<SessionSpeaker> SessionSpeakers { get; set; }

        public virtual ICollection<SessionAttendee> SessionAttendees { get; set; }

        public Track Track { get; set; }
    }
}
```
6. Add a `Track.cs` class with the following code:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Data
{
    public class Track : ConferenceDTO.Track
    {
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
```
7. Modify the `Speaker.cs` class we wrote previously to make the following two changes: update to the namespace to match our directory rename, and add a referece to the `SessionSpeaker` composite class:

```csharp
using System;
using System.Collections.Generic;

namespace BackEnd.Data
{
    public class Speaker : ConferenceDTO.Speaker
    {
        public virtual ICollection<SessionSpeaker> SessionSpeakers { get; set; } = new List<SessionSpeaker>();
    }
}
```
## Update the ApplicationDbContext

Now we need to update our `ApplicationDbContext` so Entity Framework knows about our new models.

1. Update `ApplicationDbContext.cs` to use the following code:
```csharp
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendee>()
            .HasIndex(a => a.UserName)
            .IsUnique();

             // Many-to-many: Session <-> Attendee
             modelBuilder.Entity<SessionAttendee>()
                 .HasKey(ca => new { ca.SessionId, ca.AttendeeId });

             // Many-to-many: Speaker <-> Session
             modelBuilder.Entity<SessionSpeaker>()
                 .HasKey(ss => new { ss.SessionId, ss.SpeakerId });
        }

        public DbSet<Session> Sessions { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<Speaker> Speakers { get; set; }

        public DbSet<Attendee> Attendees { get; set; }
    }
}
```
2. Fix errors due to the rename from `BackEnd.Models` to `BackEnd.Data`. You can either do this using a find / replace (replacing "BackEnd.Models" with "BackEnd.Data") or you can do a build and fix errors.
3. Ensure that the application builds now.

## Add a new database migration

**Visual Studio: Package Manager Console**

1. Run the following commands in the Package Manager Console (specify the `BackEnd` project)
```csharp
Add-Migration Refactor
Update-Database
```
**Command line**

1. Run the following commands in the command prompt in the `BackEnd` project directory:

```csharp
dotnet ef migrations add Refactor
dotnet ef database update
```
## Updating the Speakers API controller

1. Modify the query for the `GetSpeakers()` method as shown below:

```csharp
var speakers = await _context.Speakers.AsNoTracking()
                        .Include(s => s.SessionSpeakers)
                            .ThenInclude(ss => ss.Session)
                        .ToListAsync();
return speakers;
```
2. While the above will work, this is directly returning our model class. A better practice is to return an output model class. Create a `SpeakerResponse.cs` class in the `ConferenceDTO` project with the following code:

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceDTO
{
    public class SpeakerResponse : Speaker
    {
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
```
3. Now we'll add a utility method to map between these classes. In the *BackEnd* project, create an `Infrastructure` directory. Add a class named `EntityExtensions.cs` with the following mapping code:

```csharp
using BackEnd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Data
{
    public static class EntityExtensions
    {
        public static ConferenceDTO.SpeakerResponse MapSpeakerResponse(this Speaker speaker) =>
            new ConferenceDTO.SpeakerResponse
            {
                Id = speaker.Id,
                Name = speaker.Name,
                Bio = speaker.Bio,
                WebSite = speaker.WebSite,
                Sessions = speaker.SessionSpeakers?
                    .Select(ss =>
                        new ConferenceDTO.Session
                        {
                            Id = ss.SessionId,
                            Title = ss.Session.Title
                        })
                    .ToList()
            };
    }
}
```
4. Now we can update the `GetSpeakers()` method of the *SpeakersController* so that it returns our response model. Update the last few lines so that the method reads as follows:

```csharp
[HttpGet]
public async Task<ActionResult<List<ConferenceDTO.SpeakerResponse>>> GetSpeakers()
{
    var speakers = await _context.Speakers.AsNoTracking()
                                    .Include(s => s.SessionSpeakers)
                                        .ThenInclude(ss => ss.Session)
                                    .Select(s => s.MapSpeakerResponse())
                                    .ToListAsync();
    return speakers;
}
```
5. Update the `GetSpeaker()` method to use our mapped response models as follows:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ConferenceDTO.SpeakerResponse>> GetSpeaker(int id)
{
    var speaker = await _context.Speakers.AsNoTracking()
                                    .Include(s => s.SessionSpeakers)
                                        .ThenInclude(ss => ss.Session)
                                    .SingleOrDefaultAsync(s => s.Id == id);
    if (speaker == null)
    {
        return NotFound();
    }
    return speaker.MapSpeakerResponse();
}
```
6. Remove the other actions (`PutSpeaker`, `PostSpeaker`, `DeleteSpeaker`), on the `SpeakersController`.

## Adding the remaining API Controllers and DTOs

1. Add the following response DTO classes from the [save point folder](https://github.com/BekzodAkhmadov/ReferencePlanner/tree/main/ConferenceDTO)

- `AttendeeResponse`
- `SessionResponse`
- `ConferenceResponse`
- `TrackResponse`
- `TagResponse`

2. Update the `EntityExtensions` class with the `MapSessionResponse` and `MapAttendeeResponse` methods from the [save point folder](https://github.com/BekzodAkhmadov/ReferencePlanner/blob/main/BackEnd/Infrastructure/EntityExtentions.cs)

3. Copy the following controllers from [the save point folder](https://github.com/BekzodAkhmadov/ReferencePlanner/tree/main/BackEnd/Controllers) into the current project's BackEnd/Controllers directory:

- `SessionsController`
- `AttendeesController`

## Adding Conference Upload support

1. Copy the `DataLoader.cs` class from [here](https://github.com/BekzodAkhmadov/ReferencePlanner/blob/main/BackEnd/Data/DataLoader.cs) into the `Data` directory of the `BackEnd` project.

2. Copy the `SessionizeLoader.cs` and `DevIntersectionLoader.cs` classes from [here](https://github.com/BekzodAkhmadov/ReferencePlanner/tree/main/BackEnd/Data) into the current project's `/src/BackEnd/Data/` directory.

> Note: We have data loaders from the two conference series where this workshop has been presented most; you can update this to plug in your own conference file format.

3. Use the Swagger UI to upload [NDC_Sydney_2019.json](https://github.com/csharpfritz/aspnetcore-app-workshop/tree/master/src/BackEnd/Data/Import) to the /api/Sessions/upload API.










