using Xunit;

using hubitat2prom;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using Moq;
using System.Security.Principal;
using Moq.Protected;
using System.Threading;

namespace hubitat2prom.Tests;

public class MockCreator
{
    public IHttpClientFactory GetIHttpClientFactory(HttpResponseMessage? httpResponseMessage = default)
    {
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
        .ReturnsAsync(
            httpResponseMessage
            ?? new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("OK")
            }
        );
        var client = new HttpClient(handlerMock.Object);
        httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(client);
        return httpClientFactoryMock.Object;
    }
}