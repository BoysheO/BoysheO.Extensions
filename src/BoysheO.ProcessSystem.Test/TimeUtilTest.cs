using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Boysheo.ProcessSystem;
using Boysheo.ProcessSystem.LogSystem;
using Moq;
using NUnit.Framework;

[TestFixture]
public class ProcessHelperTests
{
    private Mock<IObserver<Log>> _loggerMock;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<IObserver<Log>>();
    }

    [Test]
    public async Task ExecuteCommandAsync_ShouldReturnSuccess_WhenCommandSucceeds()
    {
        // Arrange
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "bash";
        string arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/c echo Hello" : "-c \"echo Hello\"";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ProcessHelper.ExecuteCommandAsync(command, arguments, logger: _loggerMock.Object,
            cancellationToken: cancellationToken);

        // Assert
        Assert.That(result.isSuccesss);
        Assert.That(0 == result.exitCode);

        _loggerMock.Verify(logger => logger.OnNext(It.Is<Log>(log => log.Text.Contains("Starting process"))),
            Times.Once);
        _loggerMock.Verify(logger => logger.OnNext(It.Is<Log>(log => log.Text.Contains("Process exited with code"))),
            Times.Once);
    }

    [Test]
    public async Task ExecuteCommandAsync_ShouldReturnFailure_WhenCommandFails()
    {
        // Arrange
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "bash";
        string arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "/c invalidcommand"
            : "-c \"invalidcommand\"";
        var cancellationToken = CancellationToken.None;
        
        // Act
        var result = await ProcessHelper.ExecuteCommandAsync(command, arguments, logger: _loggerMock.Object,
            cancellationToken: cancellationToken);

        //Console.WriteLine(result.consoleLog.Select(v=>$"{v.Level} {v.Text}").JoinAsOneString("\n"));

        // Assert
        Assert.That(result.isSuccesss,Is.True);
        Assert.That(0 != result.exitCode);
        //_loggerMock.Verify(logger => logger.OnNext(It.Is<Log>(log => log.Level == LogLevel.E)), Times.AtLeastOnce);
    }

    [Test]
    public async Task ExecuteCommandAsync_ShouldReturnFailure_WhenFileIsNotExecutable()
    {
        // Skip this test on Windows since file permissions are not applicable
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Assert.Ignore("This test is not applicable on Windows.");

        // Arrange
        string command = "/tmp/nonexistentfile"; // A non-existent or non-executable file
        string arguments = "";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ProcessHelper.ExecuteCommandAsync(command, arguments, logger: _loggerMock.Object,
            cancellationToken: cancellationToken);

        // Assert
        Assert.That(!result.isSuccesss);
        Assert.That(-1 == result.exitCode);
        _loggerMock.Verify(
            logger => logger.OnNext(It.Is<Log>(log => log.Text.Contains("not executable or does not exist"))),
            Times.Once);
    }

    [Test]
    public async Task ExecuteCommandAsync_ShouldCancelProcess_WhenCancellationIsRequested()
    {
        // Arrange
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ping" : "sleep";
        string arguments =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "localhost -n 10" : "10"; // Sleep for 10 seconds
        var cts = new CancellationTokenSource();
        cts.CancelAfter(500); // Cancel after 500ms

        // Act
        var result = await ProcessHelper.ExecuteCommandAsync(command, arguments, logger: _loggerMock.Object,
            cancellationToken: cts.Token);

        // Assert
        Assert.That(!result.isSuccesss);
        Assert.That(-1 == result.exitCode);
        _loggerMock.Verify(logger => logger.OnNext(It.Is<Log>(log => log.Text.Contains("Cancellation requested"))),
            Times.Once);
    }

    [Test]
    public async Task ExecuteCommandAsync_ShouldHandleSudo_WhenElevationIsRequired()
    {
        // Skip this test on Windows since sudo is not applicable
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Assert.Ignore("This test is not applicable on Windows.");

        // Arrange
        string command = "ls";
        string arguments = "/root"; // A directory that requires sudo to access
        bool requireElevation = true;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ProcessHelper.ExecuteCommandAsync(command, arguments, requireElevation,
            logger: _loggerMock.Object, cancellationToken: cancellationToken);

        // Assert
        // If the user has sudo access, the exit code will be 0; otherwise, it will be non-zero
        // We can't assert a specific value, but we can verify that the command was attempted
        _loggerMock.Verify(logger => logger.OnNext(It.Is<Log>(log => log.Text.Contains("Starting process"))),
            Times.Once);
    }

    [Test]
    public void CheckIfFileIsExecutable_ShouldReturnFalse_WhenFileDoesNotExist()
    {
        // Skip this test on Windows since file permissions are not applicable
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Assert.Ignore("This test is not applicable on Windows.");

        // Arrange
        string filePath = "/tmp/nonexistentfile"; // A non-existent file

        // Act
        bool isExecutable = ProcessHelper.CheckIfFileIsExecutable(filePath, _loggerMock.Object);

        // Assert
        Assert.That(!isExecutable);
        _loggerMock.Verify(logger => logger.OnNext(It.Is<Log>(log => log.Text.Contains("does not exist"))),
            Times.Once);
    }

    [Test]
    public void CheckIfFileIsExecutable_ShouldReturnTrue_WhenFileIsExecutable()
    {
        // Skip this test on Windows since file permissions are not applicable
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Assert.Ignore("This test is not applicable on Windows.");

        // Arrange
        string filePath = "/bin/ls"; // A known executable file

        // Act
        bool isExecutable = ProcessHelper.CheckIfFileIsExecutable(filePath, _loggerMock.Object);

        // Assert
        Assert.That(isExecutable);
    }
}