namespace PixelCelebrateBackend.Service.Model
{
    public class EmailData(string toId, string toName, string subject, string body)
    {
        public string ToId { get; set; } = toId;
        public string ToName { get; set; } = toName;
        public string Subject { get; set; } = subject;
        public string Body { get; set; } = body;
    }
}