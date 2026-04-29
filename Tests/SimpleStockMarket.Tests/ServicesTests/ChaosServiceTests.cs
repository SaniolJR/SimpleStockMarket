using Services;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Controllers;

namespace Tests.ServicesTests;

public class ChaosServiceTests
{
    [Fact]
    public void Kill_ShouldCallTerminateWithCode1()
    {
        // Arrange
        var terminatorMock = new Mock<IChaosService>();
        var controller = new ChaosController(terminatorMock.Object);

        // Act
        var result = controller.Kill();

        // Assert
        Assert.IsType<OkResult>(result);
        
        Thread.Sleep(200); 
        terminatorMock.Verify(t => t.Terminate(1), Times.Once);
    }
}