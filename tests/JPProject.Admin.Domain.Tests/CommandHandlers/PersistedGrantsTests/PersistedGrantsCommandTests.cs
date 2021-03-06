using Bogus;
using FluentAssertions;
using IdentityServer4.Models;
using JPProject.Admin.Domain.CommandHandlers;
using JPProject.Admin.Domain.Commands.PersistedGrant;
using JPProject.Admin.Domain.Interfaces;
using JPProject.Admin.Domain.Tests.CommandHandlers.PersistedGrantsTests.Fakers;
using JPProject.Domain.Core.Bus;
using JPProject.Domain.Core.Interfaces;
using JPProject.Domain.Core.Notifications;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JPProject.Admin.Domain.Tests.CommandHandlers.PersistedGrantsTests
{
    public class PersistedGrantsCommandTests
    {
        private Faker _faker;
        private readonly CancellationTokenSource _tokenSource;
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<DomainNotificationHandler> _notifications;
        private readonly Mock<IMediatorHandler> _mediator;
        private readonly PersistedGrantCommandHandler _commandHandler;
        private readonly Mock<IPersistedGrantRepository> _persistedGrantRepository;

        public PersistedGrantsCommandTests()
        {
            _faker = new Faker();
            _tokenSource = new CancellationTokenSource();
            _uow = new Mock<IUnitOfWork>();
            _mediator = new Mock<IMediatorHandler>();
            _notifications = new Mock<DomainNotificationHandler>();
            _persistedGrantRepository = new Mock<IPersistedGrantRepository>();
            _commandHandler = new PersistedGrantCommandHandler(_uow.Object, _mediator.Object, _notifications.Object, _persistedGrantRepository.Object);
        }

        [Fact]
        public async Task Should_Remove_PersistedGrant()
        {
            var command = PersistedGrantCommandFaker.GenerateRemoveCommand().Generate();


            _persistedGrantRepository.Setup(s => s.Remove(It.Is<PersistedGrant>(s => s.Key == command.Key)));
            _persistedGrantRepository.Setup(s => s.GetGrant(It.Is<string>(s => s == command.Key))).ReturnsAsync(PersistedGrantFaker.GeneratePersstedGrant().Generate());

            _uow.Setup(v => v.Commit()).ReturnsAsync(true);

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            _uow.Verify(v => v.Commit(), Times.Once);

            result.Should().BeTrue();
        }


        [Fact]
        public async Task Should_Not_Remove_PersistedGrant_When_Key_Is_Null()
        {
            var command = new RemovePersistedGrantCommand(null);

            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            result.Should().BeFalse();
        }


        [Fact]
        public async Task Should_Not_Remove_PersistedGrant_When_Key_Doesnt_Exist()
        {
            var command = PersistedGrantCommandFaker.GenerateRemoveCommand().Generate();


            var result = await _commandHandler.Handle(command, _tokenSource.Token);

            _uow.Verify(v => v.Commit(), Times.Never);

            result.Should().BeFalse();
        }

    }
}
