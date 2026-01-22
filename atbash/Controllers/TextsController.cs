using Atbash.Api.Models;
using Atbash.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Atbash.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class TextsController : ControllerBase
{
    private readonly ITextService _textService;
    private readonly IAtbashService _cipher;
    private readonly ILoggerService _logger;

    public TextsController(
        ITextService textService,
        IAtbashService cipher,
        ILoggerService logger)
    {
        _textService = textService;
        _cipher = cipher;
        _logger = logger;
    }

    // POST: api/texts
    [HttpPost]
    public async Task<ActionResult<TextEntry>> AddText([FromBody] AddTextRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var encrypted = _cipher.Encrypt(request.Text);
        var entry = await _textService.AddTextAsync(request.Text, encrypted.Result, userId.Value);

        await _logger.LogAsync("StoreText", "API", $"TextId:{entry.Id}, Chars:{encrypted.ProcessedChars}");
        return CreatedAtAction(nameof(GetTextById), new { id = entry.Id }, entry);
    }

    // PATCH: api/texts/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateText(int id, [FromBody] UpdateTextRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var updated = await _textService.UpdateTextAsync(id, request.NewText, userId.Value);
        if (updated == null) return NotFound();

        await _logger.LogAsync("UpdateText", "API", $"TextId:{id}");
        return Ok(updated);
    }

    // DELETE: api/texts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteText(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _textService.DeleteTextAsync(id, userId.Value);
        if (!success) return NotFound();

        await _logger.LogAsync("DeleteText", "API", $"TextId:{id}");
        return NoContent();
    }

    // GET: api/texts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TextEntry>> GetTextById(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var text = await _textService.GetTextByIdAsync(id, userId.Value);
        if (text == null) return NotFound();

        return text;
    }

    // GET: api/texts
    [HttpGet]
    public async Task<ActionResult<List<TextEntry>>> GetAllTexts()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var texts = await _textService.GetAllTextsAsync(userId.Value);
        return Ok(texts);
    }

    // POST: api/texts/encrypt
    [HttpPost("encrypt")]
    public async Task<ActionResult<CipherResponse>> EncryptText([FromBody] EncryptRequest request)
    {
        try
        {
            var result = _cipher.Encrypt(request.Text);

            // ... сохранение в историю

            return new CipherResponse
            {
                Result = result.Result, // ✅ Теперь Result вместо EncryptedText
                ProcessedChars = result.ProcessedChars
            };
        }
        catch (Exception ex)
        {
            await _logger.LogAsync("EncryptError", "API", ex.Message);
            return Problem(detail: "Ошибка шифрования: " + ex.Message, statusCode: 500);
        }
    }

    // POST: api/texts/decrypt
    [HttpPost("decrypt")]
    public async Task<ActionResult<CipherResponse>> DecryptText([FromBody] DecryptRequest request)
    {
        try
        {
            var result = _cipher.Decrypt(request.Text);
            return new CipherResponse
            {
                Result = result.Result, // ✅ Теперь Result вместо DecryptedText
                ProcessedChars = result.ProcessedChars
            };
        }
        catch (Exception ex)
        {
            await _logger.LogAsync("DecryptError", "API", ex.Message);
            return Problem(detail: "Ошибка дешифрования: " + ex.Message, statusCode: 500);
        }
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int id) ? id : null;
    }
}

// DTO классы
public class AddTextRequest
{
    [Required, StringLength(10000)]
    public string Text { get; set; } = string.Empty;
}

public class UpdateTextRequest
{
    [Required, StringLength(10000)]
    public string NewText { get; set; } = string.Empty;
}

public class EncryptRequest
{
    [Required, StringLength(10000)]
    public string Text { get; set; } = string.Empty;
    public bool SaveToHistory { get; set; } = false;
}

public class DecryptRequest
{
    [Required, StringLength(10000)]
    public string Text { get; set; } = string.Empty;
}

//public class EncryptResponse
//{
//    public string EncryptedText { get; set; } = string.Empty;
//    public int ProcessedChars { get; set; }
//}

//public class DecryptResponse
//{
//    public string DecryptedText { get; set; } = string.Empty;
//    public int ProcessedChars { get; set; }
//}
public class CipherResponse
{
    public string Result { get; set; } = string.Empty; // camelCase
    public int ProcessedChars { get; set; }
    public string Message { get; set; } = "Успешно";
}