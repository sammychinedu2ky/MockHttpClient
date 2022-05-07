using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

//using Program
namespace Tests;

internal interface IHttpClientHandlerProtectedMember
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken);
}

[TestClass]
public class TodoCustomLoggerTests
{
    private readonly string baseUri = "https://jsonplaceholder.typicode.com/todos/";


    [TestMethod]
    public void LogTodoIsCompleted()
    {
        //arrange
        var todo = new Todo
        {
            UserId = 1,
            Id = 38,
            Title = "code",
            Completed = true
        };
        var responseMessage = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(todo))
        };

        var moq = new Mock<HttpClientHandler>();

        {
        }
        moq.Protected().As<IHttpClientHandlerProtectedMember>().Setup(m =>
            m.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseMessage);
        
        /* or you can replace the code above with this
         
         moq.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()).ReturnsAsync(responseMessage);
         
         */

        var handler = moq.Object;
        var client = new HttpClient(handler);
        client.BaseAddress = new Uri(baseUri);
        //act
        var logger = new TodoCustomLogger(client, 1);


        //assert
        var expected = $"Your todo: {todo.Title} has been completed ";
        Assert.AreEqual(expected, logger.CustomLogger().Result);
    }

    [TestMethod]
    public void LogTodoIsNotCompleted()
    {
        //arrange
        var todo = new Todo
        {
            UserId = 1,
            Id = 38,
            Title = "code",
            Completed = false
        };
        var responseMessage = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(todo))
        };
        var moq = new Mock<HttpClientHandler>();
        moq.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()).ReturnsAsync(responseMessage);
        var handler = moq.Object;
        var client = new HttpClient(handler);
        client.BaseAddress = new Uri(baseUri);
        //act
        var logger = new TodoCustomLogger(client, 1);


        //assert
        var expected = $"Your todo: {todo.Title} is yet to be completed ";
        Assert.AreEqual(expected, logger.CustomLogger().Result);
    }
}