using System;
using System.IO;
using System.Threading.Tasks;
using Atbash.Api.Data;
using Atbash.Api.Models;

namespace Atbash.Api.Services
{
    public interface ILoggerService
    {
        Task LogAsync(string operation, string source, string details);
    }

    public class LoggerService : ILoggerService
    {
        private readonly ApplicationDbContext _db;
        private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, @"C:\Users\misha\source\repos\atbash\atbash\app.log");


        public LoggerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync(string operation, string source, string details)
        {
            var entry = new LogEntry
            {
                Operation = operation,
                Source = source,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                _db.Logs.Add(entry);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync(_filePath, $"DB error: {ex.Message}\n");
            }

            try
            {
                var line = $"{entry.Timestamp:O} | {operation} | {source} | {details}{Environment.NewLine}";
                await File.AppendAllTextAsync(_filePath, line);
            }
            catch { }
        }
    }
}
