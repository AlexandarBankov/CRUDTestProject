using CRUDTestProject.Data;
using CRUDTestProject.Data.Repositories;
using CRUDTestProject.Data.Repositories.Implementation;
using CRUDTestProject.Middleware;
using CRUDTestProject.Scheduling;
using CRUDTestProject.Services;
using CRUDTestProject.Services.Implementation;
using EasyCronJob.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SoftDeleteInterceptor>();

var connectionString = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    options
    .UseSqlServer(connectionString)
    .AddInterceptors(serviceProvider.GetRequiredService<SoftDeleteInterceptor>()));

var configuration = builder.Configuration;
// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:Audience"],
        ValidIssuer = configuration["JWT:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageHandler, MessageHandler>();
builder.Services.AddScoped<IBadWordsRepository, BadWordsRepository>();
builder.Services.AddScoped<IBadWordsHandler, BadWordsHandler>();
builder.Services.AddScoped<IBulkMessagesRepository, BulkMessagesRepository>();
builder.Services.AddScoped<IBulkMessagesHandler, BulkMessagesHandler>();
builder.Services.AddScoped<IBackupRepository, BackupRepository>();
builder.Services.AddScoped<IBackupHandler, BackupHandler>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.ApplyResulation<RemoveOldSoftDeletedMessages>(options =>
{
    options.CronExpression = "0 2 * * *"; // every day at 02:00 AM
    options.TimeZoneInfo = TimeZoneInfo.Local;
    options.CronFormat = Cronos.CronFormat.Standard;
});

builder.Services.ApplyResulation<BackupMessagesDb>(options =>
{
    options.CronExpression = "0 4 1 */3 *"; // on the 1st day of the month every 3 months at 04:00 AM
    options.TimeZoneInfo = TimeZoneInfo.Local;
    options.CronFormat = Cronos.CronFormat.Standard;
});

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }