    using Atbash.Api.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace Atbash.Api.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class LogsController : ControllerBase
        {
            private readonly ApplicationDbContext _db;

            public LogsController(ApplicationDbContext db)
            {
                _db = db;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var logs = await _db.Logs.OrderByDescending(l => l.Timestamp).ToListAsync();
                return Ok(logs);
            }
        }
    }
