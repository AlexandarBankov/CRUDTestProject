using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models.Response;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json;

namespace MessagesTests.Integration
{
    public class MessageApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient client;

        private List<Message> getMessages()
        {
            return
            [
                new (){ Content = "1c", CreationDate = DateTime.Now.AddDays(-1), Email = "1e", Name = "1n", Username = "1u"},
                new (){ Content = "2c", CreationDate = DateTime.Now.AddDays(-2), Email = "2e", Name = "2n", Username = "2u"},
                new (){ Content = "3c", CreationDate = DateTime.Now.AddDays(-3), Email = "3e", Name = "3n", Username = "3u"},
                new (){ Content = "4c", CreationDate = DateTime.Now.AddDays(-4), Email = "4e", Name = "4n", Username = "4u"},
                new (){ Content = "5c", CreationDate = DateTime.Now.AddDays(-5), Email = "5e", Name = "5n", Username = "5u"}
            ];
        }

        public MessageApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            client = factory.CreateClient();
            using (var scope = factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.UpdateRange(getMessages());
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetMessagesWithoutFilters()
        {
            
            var response = await client.GetAsync("/api/messages");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var messageList = JsonConvert.DeserializeObject<List<MessageResponseModel>>(result);

            Assert.Equal(getMessages().Count, messageList.Count);
        }

    }
}
