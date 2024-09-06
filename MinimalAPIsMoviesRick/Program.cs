
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using MinimalAPIsMoviesRick.EndPoints;
using MinimalAPIsMoviesRick.Entities;
using MinimalAPIsMoviesRick.Repositories;
using MinimalAPIsMoviesRick.Services;
using System.Windows.Input;

var builder = WebApplication.CreateBuilder(args);
    


var lastName = builder.Configuration.GetValue<string>("lastName");


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

builder.Services.AddScoped<IGenresRepository, GenresRepository>();

builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();

builder.Services.AddTransient<IFileStorage,LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();    
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

 app.MapGroup("/genres").MapGenres();  
 app.MapGroup("/actors").MapActors();
 app.MapGroup("/movies").MapMovies();
app.MapGroup("/movie/{movieId:int}/comments").MapComments();


if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error;

    var error = new Error();
    error.Date = DateTime.UtcNow;
    error.ErrorMessage = exception.Message;
    error.StackTrace = exception.StackTrace;

    var repository = context.RequestServices.GetRequiredService<IErrorsRepository>();
    await repository.Create(error);

    await Results.BadRequest(new {
        type = "error",
        message = "an Unexpected expection has occured Rick",
        status=500, }).ExecuteAsync(context);

}
));
app.UseStatusCodePages();
app.UseStaticFiles();
app.UseCors();

app.UseOutputCache();
app.UseAuthorization();

app.MapGet("/", () => "Hello World! " +lastName);
app.MapGet("/error", () =>
{
    throw new InvalidOperationException("example error");
});



app.Run();


