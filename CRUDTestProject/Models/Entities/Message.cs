namespace CRUDTestProject.Models.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Content { get; set; }

    }
}
