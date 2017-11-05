using Moq;
using Service;

namespace Business.Tests
{
    public static class LogServiceMock
    {
        /// <summary>
        /// Simulates calling Log(). Doesn't really do anything at all except
        /// allows Moq to track it so we can verify it is called.
        /// </summary>
        /// <param name="mock">Extension</param>
        /// <param name="message">Simulated exception message</param>
        public static Mock<ILogService> Log_Mock(this Mock<ILogService> mock, string message)
        {
            mock.Setup(m => m.Log(It.IsAny<string>()));
            return mock;
        }
    }
}
