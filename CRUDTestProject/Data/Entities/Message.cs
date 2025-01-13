namespace CRUDTestProject.Data.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Content { get; set; }

        public required DateTime CreationDate { get; set; }

        //Data of the poster
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
