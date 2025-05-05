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

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(); // Enable CORS

// Endpoint to serve the large text file
app.MapGet("/api/FileProcessing/file-content", async (IWebHostEnvironment env, HttpContext httpContext) =>
{
    Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
    Console.WriteLine($"ContentRootPath: {env.ContentRootPath}");
    Console.WriteLine($"WebRootPath: {env.WebRootPath}");
    Console.WriteLine($"ApplicationName: {env.ApplicationName}");

    Console.WriteLine($"env.ContentRootPath: {env.ContentRootPath}");
Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
Console.WriteLine($"env.WebRootPath: {env.WebRootPath}");

var filePath = Path.Combine(env.ContentRootPath, "wap.txt");
var currentDirPath = Path.Combine(Directory.GetCurrentDirectory(), "wap.txt");
var webRootPath = Path.Combine(env.WebRootPath ?? "", "wap.txt");

Console.WriteLine($"Checking paths:");
Console.WriteLine($"1. {filePath}");
Console.WriteLine($"2. {currentDirPath}");
Console.WriteLine($"3. {webRootPath}");

Console.WriteLine($"1. File exists at ContentRootPath: {File.Exists(filePath)}");
Console.WriteLine($"2. File exists at Current Directory: {File.Exists(currentDirPath)}");
Console.WriteLine($"3. File exists at WebRootPath: {File.Exists(webRootPath)}");

if (!File.Exists(filePath))
{
    filePath = currentDirPath;
    if (!File.Exists(filePath))
    {
        filePath = webRootPath;
    }
}
    var alternateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wap.txt");
    var webRootFilePath = Path.Combine(env.WebRootPath ?? "", "wap.txt");

    Console.WriteLine($"Attempted Paths:");
    Console.WriteLine($"1. {filePath}");
    Console.WriteLine($"2. {alternateFilePath}");
    Console.WriteLine($"3. {webRootFilePath}");

    Console.WriteLine($"File Existence Checks:");
    Console.WriteLine($"1. Exists in ContentRootPath: {File.Exists(filePath)}");
    Console.WriteLine($"2. Exists in Current Directory: {File.Exists(alternateFilePath)}");
    Console.WriteLine($"3. Exists in WebRootPath: {File.Exists(webRootFilePath)}");

    string fileToUse = filePath;
    if (!File.Exists(fileToUse))
    {
        fileToUse = alternateFilePath;
        if (!File.Exists(fileToUse))
        {
            fileToUse = webRootFilePath;
        }
    }

    if (!File.Exists(fileToUse))
    {
        Console.WriteLine($"ERROR: File not found in any checked location");
        return Results.NotFound("File not found");
    }

    try 
    {
        Console.WriteLine($"Using file path: {fileToUse}");
        string fileContent = await File.ReadAllTextAsync(fileToUse);
        Console.WriteLine($"Read content length: {fileContent.Length} characters");
        return Results.Text(fileContent);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading file: {ex.Message}");
        return Results.StatusCode(500);
    }
})
;

app.Run();
