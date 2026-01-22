using Atbash.Api.Data;
using Atbash.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Atbash.Api.Services;

public interface ITextService
{
    Task<TextEntry> AddTextAsync(string originalText, string encryptedText, int userId);
    Task<TextEntry?> UpdateTextAsync(int id, string newText, int userId);
    Task<bool> DeleteTextAsync(int id, int userId);
    Task<TextEntry?> GetTextByIdAsync(int id, int userId);
    Task<List<TextEntry>> GetAllTextsAsync(int userId);
}

public class TextService : ITextService
{
    private readonly ApplicationDbContext _db;
    private readonly IAtbashService _cipher;

    public TextService(ApplicationDbContext db, IAtbashService cipher)
    {
        _db = db;
        _cipher = cipher;
    }

    public async Task<TextEntry> AddTextAsync(string originalText, string encryptedText, int userId)
    {
        var entry = new TextEntry
        {
            OriginalText = originalText,
            EncryptedText = encryptedText,
            UserId = userId
        };

        _db.Texts.Add(entry);
        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task<TextEntry?> UpdateTextAsync(int id, string newText, int userId)
    {
        var entry = await _db.Texts.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (entry == null) return null;

        entry.OriginalText = newText;
        entry.EncryptedText = _cipher.Encrypt(newText).Result;
        entry.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task<bool> DeleteTextAsync(int id, int userId)
    {
        var entry = await _db.Texts.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (entry == null) return false;

        _db.Texts.Remove(entry);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<TextEntry?> GetTextByIdAsync(int id, int userId)
    {
        return await _db.Texts.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
    }

    public async Task<List<TextEntry>> GetAllTextsAsync(int userId)
    {
        return await _db.Texts
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}