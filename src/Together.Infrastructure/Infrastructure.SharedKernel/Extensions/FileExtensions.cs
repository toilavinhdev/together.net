using Microsoft.AspNetCore.Http;

namespace Infrastructure.SharedKernel.Extensions;

public static class FileExtensions
{
    public static string GetExtension(this IFormFile file) => Path.GetExtension(file.FileName);
    
    public static string ReadAllText(this string path) => File.ReadAllText(path);

    public static string NormalizeFileName(this string fileName) => fileName.Replace("\\", "/");

    public static string MakeDirectory(this string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }
}