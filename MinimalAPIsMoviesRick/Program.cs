
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIsMoviesRick.EndPoints;
using MinimalAPIsMoviesRick.Entities;
using MinimalAPIsMoviesRick.Repositories;
using MinimalAPIsMoviesRick.Services;
using MinimalAPIsMoviesRick.Servicesp;
using MinimalAPIsMoviesRick.Utilities;
using System.Security.Cryptography.Xml;
using System.Windows.Input;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserStore<IdentityUser>,UserStore>();
builder.Services.AddIdentityCore<IdentityUser>();
builder.Services.AddTransient<SignInManager<IdentityUser>>();

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
builder.Services.AddScoped<IUsersRepository, UsersRepository>();


builder.Services.AddTransient<IFileStorage,LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();
builder.Services.AddAuthentication().AddJwtBearer(Options =>
Options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.Zero,
    IssuerSigningKeys = KeysHandler.GetAllKeys(builder.Configuration),
  //  IssuerSigningKey = KeysHandler.GetKey(builder.Configuration).First()
});

builder.Services.AddAuthorization();

var app = builder.Build();

 app.MapGroup("/genres").MapGenres();  
 app.MapGroup("/actors").MapActors();
 app.MapGroup("/movies").MapMovies();
app.MapGroup("/movie/{movieId:int}/comments").MapComments();
app.MapGroup("/users").MapUsers();

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


