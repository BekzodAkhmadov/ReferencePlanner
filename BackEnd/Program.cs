using BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;
using static BackEnd.Controllers.SessionsController;

internal partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Conference Planner API v1",
                Version = "v1"
            });

            // Include XML Comments for API Documentation
            options.MapType<ConferenceFormat>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames(typeof(ConferenceFormat))
            .Select(name => new OpenApiString(name))
            .ToList<IOpenApiAny>()
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

        });



        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) 
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
              options.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Planner API v1"));
        }

        app.UseHttpsRedirection();


        app.UseAuthorization();

        app.MapControllers();

        app.Run();


    }


}