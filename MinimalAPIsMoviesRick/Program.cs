
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
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

var genresEndpoints = app.MapGroup("/genres");



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

genresEndpoints.MapGet("", GetGenre).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

genresEndpoints.MapGet("/{id:int}", GetById);

//Data from SQL
genresEndpoints.MapPost("/", Create);

genresEndpoints.MapPut("/{id:int}", Update);

genresEndpoints.MapDelete("/{id:int}", Delete);
//Middlewares Zone - End

//Endpoints End

app.Run();


//Lamda Expressions to Named Methods
static async Task<Ok<List<Genre>>> GetGenre(IGenresRepository repository) 
{   
    var geners = await repository.GetAll();
    return TypedResults.Ok(geners);
}

static async Task<Results<Ok<Genre>,NotFound>> GetById (int id, IGenresRepository genresRepository) {

    var genre = await genresRepository.GetById(id);

    if (genre is null)
    {
         return TypedResults.NotFound();

        // return Results.NotFound();
    }
    // return Results.Ok(genre);
    return TypedResults.Ok(genre);

}

static async Task<Created<Genre>> Create (Genre genre, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore) {
    var id = await genresRepository.Create(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    // return TypedResults.Ok(genre);
    return TypedResults.Created($"/genres/{id}", genre);


}


static async Task<Results<NotFound,NoContent>>  Update (int id, Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore)  {

    var exists = await repository.Exists(id);

    if (!exists)
    {
        return TypedResults.NotFound();

    }

    await repository.Update(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.NoContent();
}

static async Task<Results<NotFound,NoContent>> Delete (int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
{
    var exists = await repository.Exists(id);

    if (!exists)
    {
        return TypedResults.NotFound();

    }

    await repository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.NoContent();
}

//Lamda Expressions to Named Methods - End
