namespace CRUDTestProject.Data.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Content { get; set; }

        public required DateTime CreationDate { get; set; }

        public required virtual User User { get; set; }

    }
}
