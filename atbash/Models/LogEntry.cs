using System;

namespace Atbash.Api.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Operation { get; set; } = "";
        public string Source { get; set; } = "";
        public string Details { get; set; } = "";
    }
}
