using CRUDTestProject.Data.Entities;
using CRUDTestProject.Services;
using Moq;

namespace MessagesTests.Controller
{
    public class RepositoryFixture
    {
        public Mock<IMessageHandler> Mock { get; private set; }
        public Guid ExistingId { get; private set; }
        public Guid MissingId { get; private set; }

        public Message Message { get; private set; }

        public RepositoryFixture()
        {
            ExistingId = Guid.NewGuid();
            MissingId = Guid.NewGuid();
            Message = new()
            {
                Id = this.ExistingId,
                Name = "string",
                Content = "string",
                Username = "string",
                Email = "string",
                CreationDate = DateTime.Now
            };

            Mock = new();
            Mock.Setup(repository => repository.GetById(ExistingId)).Returns(Message);

            Message? m = null;
            Mock.Setup(repository => repository.GetById(MissingId)).Returns(m);

            Mock.Setup(repository => repository.Insert(It.IsAny<Message>()));

            Mock.Setup(repository => repository.Delete(It.IsAny<Guid>(), It.IsAny<string>()));

            Mock.Setup(repository => repository.Update(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Message);
        }
    }
}
