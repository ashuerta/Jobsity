using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jobsity.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using Jobsity.Core.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Jobsity.Service.Controllers.Tests
{
    [TestClass()]
    public class BotControllerTests
    {
        private IFixture _fixture;
        private JobsityMessage _mockEntity;
        private Mock<ILogger<BotController>> _mockLogger;
        private Mock<IMemoryCache> _mockMemoryCache;
        private Mock<IWebHostEnvironment> _mockWebHosting;

        [TestInitialize]
        public void BeforeEach()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customize<JobsityMessage>(ob => ob
                .With(t => t.User, "ashuerta")
                .With(t => t.Msg, "/stock=aapl.us")
                .With(t => t.Date, DateTime.Now)
                .WithAutoProperties());

            _mockEntity = _fixture.Create<JobsityMessage>();
            _mockLogger = new Mock<ILogger<BotController>>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _mockWebHosting = new Mock<IWebHostEnvironment>();
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("Jobsity.ServiceTests\\bin\\Debug\\netcoreapp3.0", "\\Jobsity.Service\\wwwroot"));
            _mockWebHosting.Setup(s => s.WebRootPath).Returns(path);
        }

        [TestMethod()]
        public async Task ResponseMsgAsyncTestAsync()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            var controller = new BotController(_mockLogger.Object, memoryCache, _mockWebHosting.Object);

            // Act
            
            var actionResult = await controller.ResponseMsgAsync(_mockEntity);


            // Assert
            Xunit.Assert.NotNull(actionResult);
            var response = Xunit.Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Xunit.Assert.NotNull(response);
            Xunit.Assert.Equal((int)HttpStatusCode.OK, response.StatusCode.Value);
            Xunit.Assert.NotNull(response.Value);
        }
    }
}