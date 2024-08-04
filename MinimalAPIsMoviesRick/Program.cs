
using Microsoft.AspNetCore.Builder;
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




//Middlewares Zone - Begin

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseCors();

app.UseOutputCache();

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

app.MapGet("/genres",async (IGenresRepository genresRepository) =>
{   
    return await genresRepository.GetAll(); 

}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

app.MapGet("/genres/{id:int}", async (int id,IGenresRepository genresRepository) =>{ 

    var genre = await genresRepository.GetById(id);

    if (genre is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(genre);
 
}
);

//Data from SQL
app.MapPost("/genres", async (Genre genre,IGenresRepository genresRepository,IOutputCacheStore outputCacheStore) => { 
    var id=  await genresRepository.Create(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
   // return TypedResults.Ok(genre);
    return TypedResults.Created($"/genres/{id}",genre);


});

app.MapPut("/genres/{id:int}", async (int id, Genre genre,IGenresRepository repository, IOutputCacheStore  outputCacheStore) => {

    var exists = await repository.Exists(id);

    if (!exists) {
        return Results.NotFound();
    
    }

    await repository.Update(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);                                                  
    return Results.NoContent();
});

app.MapDelete("/genres/{id:int}", async (int id, IGenresRepository repository, IOutputCacheStore outputCacheStore) =>
{
    var exists = await repository.Exists(id); 
    
    if (!exists) {
        return Results.NotFound();
    
    }

    await repository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.NoContent();
});
//Middlewares Zone - End
app.Run();
