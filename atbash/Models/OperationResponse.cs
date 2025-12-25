namespace Atbash.Api.Models
{
    public class OperationResponse
    {
        public string Result { get; set; } = "";
        public int ProcessedChars { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public string Message { get; set; } = "";
    }
}
