using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(); // Enable CORS

// Endpoint to serve the large text file
app.MapGet("/api/FileProcessing/file-content", async (IWebHostEnvironment env, HttpContext httpContext) =>
{
    var filePath = Path.Combine(env.ContentRootPath, "wap_final.txt");
    
    if (!File.Exists(filePath))
    {
        Console.WriteLine($"ERROR: File not found at {filePath}");
        return Results.NotFound("File not found");
    }

    try 
    {
        string fileContent = await File.ReadAllTextAsync(filePath);
        
        Console.WriteLine($"Read content length: {fileContent.Length} characters");

        return Results.Ok(fileContent);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading file: {ex.Message}");
        return Results.StatusCode(500);
    }
})
.WithName("GetFileContent")
.WithOpenApi();

app.Run();
