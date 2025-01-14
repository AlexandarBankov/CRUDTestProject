﻿using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using Moq;

namespace MessagesTests.Controller
{
    public class RepositoryFixture 
    {
        public Mock<IMessageRepository> Mock { get; private set; }
        public IMessageRepository Repository { get; private set; }
        public Guid ExistingId { get; private set; }
        public Guid MissingId { get; private set; }

        public Message message { get; private set; }

        public RepositoryFixture()
        {
            ExistingId = Guid.NewGuid();
            MissingId = Guid.NewGuid();
            message = new() 
            {
                Id = this.ExistingId,
                Name = "string",
                Content = "string",
                Username = "string",
                Email = "string",
                CreationDate = DateTime.Now
            };
            
            Mock = new();
            Mock.Setup(repository => repository.GetById(ExistingId)).Returns(message);
            
            Message? m = null;
            Mock.Setup(repository => repository.GetById(MissingId)).Returns(m);
            
            Mock.Setup(repository => repository.Insert(It.IsAny<Message>()));
            
            Repository = Mock.Object;
        }
    }
}
