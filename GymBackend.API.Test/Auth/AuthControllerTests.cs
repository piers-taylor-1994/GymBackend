using GymBackend.API.Models;
using GymBackend.Controllers;
using GymBackend.Core.Contracts.Auth;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Diagnostics.CodeAnalysis;

namespace GymBackend.API.Test.Auth
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class AuthTests
    {
        private ILogger<AuthController> _logger;
        private IAuthManager _authManager;
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<AuthController>>();
            _authManager = Substitute.For<IAuthManager>();
            _authController = new AuthController(_logger, _authManager);
        }

        [Test]
        public void Logon_EmptyLogonDetails_ThrowsException()
        {
            //Arrange
            var logon = new Logon { Username = "", Password = "" };

            //Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _authController.Logon(logon));

            //Assert
            Assert.That(ex.Message, Is.EqualTo("Logon details missing"));
        }

        [Test]
        public async Task Logon_WrongLogonDetails_ReturnsNull()
        {
            //Arrange
            var logon = new Logon { Username = "123", Password = "123" };

            //Act
            var result = await _authController.Logon(logon);

            //Assert
            Assert.That(result.Value, Is.EqualTo(null));
        }
    }
}
