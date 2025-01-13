using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Models
{
    public class MessageFilterParameters
    {
        public string? SearchInName { get; set; }
        public string? SearchInContent { get; set; }
        public DateTime? CreationIsAfter { get; set; }
        public DateTime? CreationIsBefore { get; set; }

        public IEnumerable<Message> FilterMessages(IEnumerable<Message> messages) 
        {
            if (SearchInContent is not null)
            {
                messages = messages.Where(m => m.Content.Contains(SearchInContent));
            }
            if (SearchInName is not null)
            {
                messages = messages.Where(m => m.Name.Contains(SearchInName));
            }
            if (CreationIsAfter is not null)
            {
                messages = messages.Where(m => m.CreationDate > CreationIsAfter);
            }
            if (CreationIsBefore is not null)
            {
                messages = messages.Where(m => m.CreationDate < CreationIsBefore);
            }

            return messages;
        }
    }
}
