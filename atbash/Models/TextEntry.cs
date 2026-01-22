using System.ComponentModel.DataAnnotations.Schema;

namespace Atbash.Api.Models;

public class TextEntry
{
    public int Id { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public string EncryptedText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}