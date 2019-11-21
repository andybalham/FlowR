namespace BusinessExample.Core.Entities
{
    public class EmailCommunication
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string EmailAddress { get; set; }
        public EmailCommunicationStatus? Status { get; set; }
    }

    public enum EmailCommunicationStatus
    {
        Pending,
        Sent,
        Failed
    }
}
