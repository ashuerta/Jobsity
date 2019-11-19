using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jobsity.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using Jobsity.Core.Entity;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.Extensions.Logging;
using AutoFixture.AutoMoq;
using Jobsity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RestSharp;
using System.Linq;

namespace Jobsity.Service.Controllers.Tests
{
    [TestClass()]
    public class SecurityControllerTests
    {
        private IFixture _fixture;
        private JobsityUser _mockEntity;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IConfigurationSection> _mockSectionConnection;
        private Mock<ILogger<SecurityController>> _mockLogger;
        private Mock<UserManager<JobsityUser>> _mockUserManager;
        private Mock<JobsityContext> _mockContext;

        [TestInitialize]
        public void BeforeEach()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customize<JobsityUser>(ob => ob
                .With(t => t.UserName, "Khalel")
                .With(t => t.Email, "khalel@hotmail.com")
                .With(t => t.Password, "Jobsity@123")
                .With(t => t.SecurityStamp, Guid.NewGuid().ToString())
                .WithAutoProperties());

            _mockEntity = _fixture.Create<JobsityUser>();

            _mockSectionConnection = new Mock<IConfigurationSection>();
            //If you want to connect to local SQL Server
            _mockSectionConnection.Setup(s => s.Value).Returns(@"Server=ABEYSSD\\SQL2017;Initial catalog=jobsityDev;User id=sa;Password=sql2017;");

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c.GetSection("ConnectionStrings:Jobsity")).Returns(_mockSectionConnection.Object);

            var options = new DbContextOptionsBuilder<JobsityContext>()
                .UseSqlServer(_mockConfiguration.Object.GetSection("ConnectionStrings:Jobsity").Value)
                .Options;
            _mockContext = new Mock<JobsityContext>(new object[] { options });

            var mockUserStore = new Mock<IUserStore<JobsityUser>>();
            _mockUserManager = new Mock<UserManager<JobsityUser>>(MockBehavior.Default, new object[] { mockUserStore.Object, null, null, null, null, null, null, null, null });

            _mockLogger = new Mock<ILogger<SecurityController>>();
        }

        [TestMethod()]
        public async Task LoginAsyncTestAsync()
        {
            // Arrange
            _fixture.Customize<JobsityUser>(ob => ob
                .With(t => t.UserName, "ashuerta")
                .With(t => t.Email, "ashuerta@hotmail.com")
                .With(t => t.Password, "Jobsity@123")
                .With(t => t.SecurityStamp, Guid.NewGuid().ToString())
                .WithAutoProperties());

            var ju = _fixture.Create<JobsityUser>();

            _mockUserManager.Setup(x => x.CheckPasswordAsync(ju, "Jobsity@123"))
                .Returns(Task.FromResult(true)).Verifiable();
            _mockUserManager.Setup(x => x.FindByNameAsync("ashuerta"))
                .Returns(Task.FromResult(ju)).Verifiable();

            var mockRepository = new Mock<SecurityRepository>(new object[] { _mockContext.Object });
            var controller = new SecurityController(_mockLogger.Object, mockRepository.Object, _mockUserManager.Object); 
            // Act
            var actionResult = await controller.LoginAsync(ju);


            // Assert
            Xunit.Assert.NotNull(actionResult);
            var response = Xunit.Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Xunit.Assert.NotNull(response);
            Xunit.Assert.Equal((int)HttpStatusCode.OK, response.StatusCode.Value);
            Xunit.Assert.NotNull(response.Value);
        }

        [TestMethod()]
        public async Task RegisterAsyncTestAsync()
        {
            // Arrange
            _mockUserManager.Setup(x => x.CreateAsync(_mockEntity, "Jobsity@123"))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            var mockRepository = new Mock<SecurityRepository>(new object[] { _mockContext.Object });
            var controller = new SecurityController(_mockLogger.Object, mockRepository.Object, _mockUserManager.Object);

            // Act
            var actionResult = await controller.RegisterAsync(_mockEntity);

            // Assert
            Xunit.Assert.NotNull(actionResult);
            var response = Xunit.Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Xunit.Assert.NotNull(response);
            Xunit.Assert.Equal((int)HttpStatusCode.OK, response.StatusCode.Value);
            Xunit.Assert.NotNull(response.Value);
        }
    }
}