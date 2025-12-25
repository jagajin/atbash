using Microsoft.AspNetCore.Mvc;
using Atbash.Api.Models;
using Atbash.Api.Services;

namespace Atbash.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CipherController : ControllerBase
    {
        private readonly IAtbashService _cipher;
        private readonly ILoggerService _logger;

        public CipherController(IAtbashService cipher, ILoggerService logger)
        {
            _cipher = cipher;
            _logger = logger;
        }

        [HttpPost("encrypt")]
        public async Task<ActionResult<OperationResponse>> Encrypt([FromBody] OperationRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Text))
                return BadRequest("Text required");

            var res = _cipher.Encrypt(req.Text);
            await _logger.LogAsync("Encrypt", req.Source, $"Chars:{res.ProcessedChars}");
            return Ok(res);
        }

        [HttpPost("decrypt")]
        public async Task<ActionResult<OperationResponse>> Decrypt([FromBody] OperationRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Text))
                return BadRequest("Text required");

            var res = _cipher.Decrypt(req.Text);
            await _logger.LogAsync("Decrypt", req.Source, $"Chars:{res.ProcessedChars}");
            return Ok(res);
        }
    }
}
