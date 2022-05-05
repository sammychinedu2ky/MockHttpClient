using System.Text.Json;
using System.Text.Json.Serialization;

var httpClient = new HttpClient {BaseAddress = new Uri("https://jsonplaceholder.typicode.com/todos/")};
var logger = new TodoCustomLogger(httpClient, 1);
await logger.CustomLogger();

public class Todo
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("userId")] public int UserId { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; } = "";

    [JsonPropertyName("completed")] public bool Completed { get; set; }
}

public class TodoCustomLogger
{
    private readonly HttpClient _client;

    public TodoCustomLogger(HttpClient client, int id)
    {
        _client = client;
        Id = id;
    }

    private int Id { get; }

    public async Task<string> CustomLogger()
    {
        var response = await _client.GetAsync(Convert.ToString(Id));
        var responseBody = await response.Content.ReadAsStringAsync();
        var deserialized = JsonSerializer.Deserialize<Todo>(responseBody)!;
        return deserialized.Completed
            ? $"Your todo: {deserialized.Title} has been completed "
            : $"Your todo: {deserialized.Title} is yet to be completed ";
    }
}


public class CustomHandler : HttpClientHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            Content = new StringContent(
                "{ 'userId': 1,  'id': 38,  'title': 'fugiat veniam minus',   'completed': false }")
        });
    }
}