namespace CRUDTestProject.Data.Entities
{
    public class Message : ISoftDeletable
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Content { get; set; }
        public required DateTime CreationDate { get; set; }

        //Data of the poster
        public required string Username { get; set; }
        public required string Email { get; set; }


        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
