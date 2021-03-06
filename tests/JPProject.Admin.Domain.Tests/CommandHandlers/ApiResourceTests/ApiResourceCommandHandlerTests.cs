using Bogus;
using FluentAssertions;
using IdentityServer4.Models;
using JPProject.Admin.Domain.CommandHandlers;
using JPProject.Admin.Domain.Commands.ApiResource;
using JPProject.Admin.Domain.Interfaces;
using JPProject.Admin.Fakers.Test.ApiResourceFakers;
using JPProject.Domain.Core.Bus;
using JPProject.Domain.Core.Interfaces;
using JPProject.Domain.Core.Notifications;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JPProject.Admin.Domain.Tests.CommandHandlers.ApiResourceTests
{
    public class ApiResourceCommandHandlerTests
    {
        private readonly ApiResourceCommandHandler _commandHandler;
        private readonly Mock<DomainNotificationHandler> _notifications;
        private readonly Mock<IMediatorHandler> _mediator;
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<IApiResourceRepository> _apiResourceRepository;
        private readonly CancellationTokenSource _tokenSource;
        private readonly Faker _faker;
        public ApiResourceCommandHandlerTests()
        {
            _faker = new Faker();
            _tokenSource = new CancellationTokenSource();
            _uow = new Mock<IUnitOfWork>();
            _mediator = new Mock<IMediatorHandler>();
            _notifications = new Mock<DomainNotificationHandler>();
            _apiResourceRepository = new Mock<IApiResourceRepository>();
            _commandHandler = new ApiResourceCommandHandler(_uow.Object, _mediator.Object, _notifications.Object, _apiResourceRepository.Object);
        }


        [Fact]
        public async Task Should_Not_Save_Resource_When_It_Already_Exist()
        {
            var command = ResourceCommandFaker.GenerateRegisterApiResourceCommand().Generate();

            _apiResourceRepository.Setup(s => s.GetResource(It.Is<string>(q => q == command.Resource.Name))).ReturnsAsync(EntityApiResourceFaker.GenerateResource().Generate());

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            result.Should().BeFalse();
            _apiResourceRepository.Verify(s => s.GetResource(It.Is<string>(q => q == command.Resource.Name)), Times.Once);
            _uow.Verify(v => v.Commit(), Times.Never);

        }

        [Fact]
        public async Task Should_Save_Resource()
        {
            var command = ResourceCommandFaker.GenerateRegisterApiResourceCommand().Generate();
            _apiResourceRepository.Setup(s => s.GetResource(It.Is<string>(q => q == command.Resource.Name))).ReturnsAsync((ApiResource)null);
            _apiResourceRepository.Setup(s => s.Add(It.Is<ApiResource>(i => i.Name == command.Resource.Name)));
            _uow.Setup(s => s.Commit()).ReturnsAsync(true);

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            _apiResourceRepository.Verify(s => s.Add(It.IsAny<ApiResource>()), Times.Once);
            _apiResourceRepository.Verify(s => s.GetResource(It.Is<string>(q => q == command.Resource.Name)), Times.Once);

            result.Should().BeTrue();
        }


        [Fact]
        public async Task Should_Not_Update_Resource_When_It_Doesnt_Exist()
        {
            var command = ResourceCommandFaker.GenerateUpdateApiResourceCommand().Generate();
            _apiResourceRepository.Setup(s => s.GetResource(It.Is<string>(q => q == command.OldResourceName))).ReturnsAsync(EntityApiResourceFaker.GenerateResource().Generate());

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            result.Should().BeFalse();
            _apiResourceRepository.Verify(s => s.GetResource(It.Is<string>(q => q == command.OldResourceName)), Times.Once);
        }


        [Fact]
        public async Task Should_Not_Update_Resource_When_Name_Isnt_Provided()
        {
            var command = ResourceCommandFaker.GenerateUpdateApiResourceCommand().Generate();
            command.Resource.Name = null;

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            result.Should().BeFalse();
            _uow.Verify(v => v.Commit(), Times.Never);

        }

        [Fact]
        public async Task Should_Update_Resource()
        {
            var oldResourceName = "old-resource-name";
            var command = ResourceCommandFaker.GenerateUpdateApiResourceCommand(oldResourceName: oldResourceName).Generate();
            _apiResourceRepository.Setup(s => s.GetResource(It.Is<string>(q => q == oldResourceName))).ReturnsAsync(EntityApiResourceFaker.GenerateResource().Generate());
            _apiResourceRepository.Setup(s => s.UpdateWithChildrens(It.Is<string>(s => s == command.OldResourceName), It.Is<ApiResource>(i => i.Name == command.Resource.Name))).Returns(Task.CompletedTask);
            _uow.Setup(s => s.Commit()).ReturnsAsync(true);

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            _apiResourceRepository.Verify(s => s.UpdateWithChildrens(It.Is<string>(s => s == command.OldResourceName), It.IsAny<ApiResource>()), Times.Once);
            _apiResourceRepository.Verify(s => s.GetResource(It.Is<string>(q => q == oldResourceName)), Times.Once);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Not_Remove_Resource_When_Name_Isnt_Provided()
        {
            var command = new RemoveApiResourceCommand(null);

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            result.Should().BeFalse();
            _uow.Verify(v => v.Commit(), Times.Never);

        }


        [Fact]
        public async Task Should_Not_Remove_Resource_When_It_Doesnt_Exist()
        {

            var command = ResourceCommandFaker.GenerateUpdateApiResourceCommand().Generate();

            _apiResourceRepository.Setup(s => s.GetResource(It.Is<string>(q => q == command.OldResourceName))).ReturnsAsync((ApiResource)null);

            var result = await _commandHandler.Handle(command, _tokenSource.Token);


            result.Should().BeFalse();
            _uow.Verify(v => v.Commit(), Times.Never);
            _apiResourceRepository.Verify(s => s.GetResource(It.Is<string>(q => q == command.OldResourceName)), Times.Once);
        }

        [Fact]
        public async Task Should_Remove_Resource()
        {
            var command = ResourceCommandFaker.GenerateRemoveApiResourceCommand().Generate();
            _apiResourceRepository.Setup(s => s.GetResource(It.Is<string>(q => q == command.Resource.Name))).ReturnsAsync(EntityApiResourceFaker.GenerateResource().Generate());
            _apiResourceRepository.Setup(s => s.Remove(It.IsAny<ApiResource>()));

            _uow.Setup(s => s.Commit()).ReturnsAsync(true);

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            _apiResourceRepository.Verify(s => s.GetResource(It.Is<string>(q => q == command.Resource.Name)), Times.Once);
            _apiResourceRepository.Verify(s => s.Remove(It.IsAny<ApiResource>()), Times.Once);

            result.Should().BeTrue();
        }
    }
}
