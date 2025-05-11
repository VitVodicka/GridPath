using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Controllers;

namespace TestProject2
{
    public class UnitTest1
    {
        private readonly ILogger<HomeController> _logger;

        public UnitTest1()
        {
            // Vytvoření loggeru (pro jednoduchost bez mockovacího frameworku)
            _logger = new LoggerFactory().CreateLogger<HomeController>();
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(_logger);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
