using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Logger
{
    public static class LoggerExtensions
    {
        public static Mock<ILogger<T>> VerifyLogWasCalled<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, string expectedMessage, Exception e = null)
        {
            logger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => Equals(v.ToString(), expectedMessage)),
                    e,
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            return logger;
        }
    }
}
