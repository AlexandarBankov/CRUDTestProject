using System.ComponentModel.DataAnnotations;

namespace CRUDTestProject.Data.Entities
{
    public class BadWord
    {
        [Key]
        public required string Word { get; set; }
    }
}
