
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using MinimalAPIsMoviesRick.EndPoints;
using MinimalAPIsMoviesRick.Entities;
using MinimalAPIsMoviesRick.Repositories;

var builder = WebApplication.CreateBuilder(args);

//Services Zone - Begin
    
builder.Services.AddScoped<IGenresRepository, GenresRepository>();

var lastName = builder.Configuration.GetValue<string>("lastName");

// Service Zone - End

if (lastName is not null )
{
    string lastNameInUpperCase = lastName.ToUpper();

}

builder.Services.AddCors(Options =>
{
    Options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["allowedOrgins"]!).AllowAnyMethod().AllowAnyHeader();
    });

    Options.AddPolicy("free", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });

});

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

 app.MapGroup("/genres").MapGenres();  // Removed var genresEndpoints =



//Middlewares Zone - Begin

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseCors();

app.UseOutputCache();


//Endpoints Starts
app.MapGet("/", () => "Hello World! " +lastName);


/* data run from local
app.MapGet("/genres", () =>
{
    var genres = new List<Genre>() {
        new Genre
        {
            Id= 1,
            Name = "Drama"

        },
        new Genre
        {
            Id= 2,
            Name ="Action"


        },
        new Genre {
            Id= 3,
            Name ="Comedy"

        }

        };

    return genres;  
}).CacheOutput(c=> c.Expire(TimeSpan.FromSeconds(15)));

*/


//Middlewares Zone - End

//Endpoints End

app.Run();

