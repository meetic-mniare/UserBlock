using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using UserBlock.Contracts;

namespace UserBlock.IntegrationTest;

[TestFixture]
public class MemoryCacheHandlerIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task Invoke_GetRequest_ReturnsCachedResponse()
    {
        // Send a GET request to the server
        await AddToken();
        var response1 = await HttpClient!.GetAsync(GetRoute);
        response1.EnsureSuccessStatusCode();
        var responseContent1 = await response1.Content.ReadAsStringAsync();

        // Send another GET request to the same endpoint
        var response2 = await HttpClient.GetAsync(GetRoute);
        response2.EnsureSuccessStatusCode();
        var responseContent2 = await response2.Content.ReadAsStringAsync();

        // Assert that the second response matches the first response (indicating cached response)
        Assert.That(responseContent2, Is.EqualTo(responseContent1));
    }

    [Test]
    public async Task Invoke_GetRequest_CacheSkippedInHeader_ReturnsNewResponse()
    {
        // Send a GET request to the server
        await AddToken();
        var response1 = await HttpClient!.GetAsync(GetRoute);
        response1.EnsureSuccessStatusCode();
        var responseContent1 = await response1.Content.ReadAsStringAsync();

        // Send another GET request to the same endpoint and skip cache
        HttpClient.DefaultRequestHeaders.Add("X-Skip-Cache", "yes");
        var response2 = await HttpClient.GetAsync(GetRoute);
        response2.EnsureSuccessStatusCode();
        var responseContent2 = await response2.Content.ReadAsStringAsync();

        HttpClient.DefaultRequestHeaders.Remove("X-Skip-Cache");

        // Assert that the second response matches the first response (indicating cached response)
        Assert.That(responseContent2, Is.Not.EqualTo(responseContent1));
    }

    [Test]
    public async Task Invoke_PostRequest_ResetsCache()
    {
        await AddToken();
        // Send a GET request to the server to populate cache
        var initialResponse = await HttpClient!.GetAsync(GetRoute);
        initialResponse.EnsureSuccessStatusCode();
        var initialResponseContent = await initialResponse.Content.ReadAsStringAsync();

        // Send a POST request to the server
        var userInfo = new UserRequest(ValidUserName2, null);
        var jsonContent = JsonContent.Create(
            userInfo,
            mediaType: new MediaTypeHeaderValue("application/json")
        );
        var postResponse = await HttpClient.PostAsync(PostRoute, jsonContent);
        postResponse.EnsureSuccessStatusCode();

        // Send a GET request again to ensure cache is reset
        var updatedResponse = await HttpClient.GetAsync(GetRoute);
        updatedResponse.EnsureSuccessStatusCode();
        var updatedResponseContent = await updatedResponse.Content.ReadAsStringAsync();

        // Assert that the updated response is different from the initial one (indicating cache reset)
        Assert.That(updatedResponseContent, Is.Not.EqualTo(initialResponseContent));
    }

    [Test]
    public async Task Invoke_DeleteRequest_ResetsCache()
    {
        await AddToken();
        var userInfo = new UserRequest(ValidUserName2, null);
        var jsonContent = JsonContent.Create(
            userInfo,
            mediaType: new MediaTypeHeaderValue("application/json")
        );

        // Send a GET request to the server to populate cache
        var initialResponse = await HttpClient!.GetAsync(GetRoute);
        initialResponse.EnsureSuccessStatusCode();
        var initialResponseContent = await initialResponse.Content.ReadAsStringAsync();

        //Add bloked user
        var blockResponse = await HttpClient.PostAsync(PostRoute, jsonContent);
        blockResponse.EnsureSuccessStatusCode();
        var blockResponseContent = await blockResponse.Content.ReadAsStringAsync();

        // Initial response content should be different from the entity after block update
        Assert.That(blockResponseContent, Is.Not.EqualTo(initialResponseContent));

        // Send a DELETE request
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(DeleteRoute, UriKind.Relative),
            Content = new StringContent(
                JsonConvert.SerializeObject(userInfo),
                Encoding.UTF8,
                "application/json"
            )
        };
        var deleteResponse = await HttpClient.SendAsync(request);
        deleteResponse.EnsureSuccessStatusCode();

        // Send a GET request again to ensure cache is reset
        var deletedEntity = await HttpClient.GetAsync(GetRoute);
        deletedEntity.EnsureSuccessStatusCode();
        var deleteUpdateContent = await deletedEntity.Content.ReadAsStringAsync();

        // Assert that the updated response is different from the previous response (indicating cache reset)
        Assert.That(deleteUpdateContent, Is.Not.EqualTo(blockResponseContent));
    }

    [Test]
    public async Task Invoke_CacheExpiration_CacheExpires()
    {
        await AddToken();
        // Send a GET request to the server to populate cache
        var initialResponse = await HttpClient!.GetAsync(GetRoute);
        initialResponse.EnsureSuccessStatusCode();
        var initialResponseContent = await initialResponse.Content.ReadAsStringAsync();
        // Wait for the cache to expire (adjust the duration as needed)
        await Task.Delay(TimeSpan.FromMinutes(0.2));

        // Send a GET request again to ensure cache has expired
        var updatedResponse = await HttpClient.GetAsync(GetRoute);
        updatedResponse.EnsureSuccessStatusCode();
        var updatedResponseContent = await updatedResponse.Content.ReadAsStringAsync();
        // Assert that the updated response is different from the initial one (indicating cache expiration)
        Assert.That(updatedResponseContent, Is.Not.EqualTo(initialResponseContent));
    }

    [Test]
    public async Task Invoke_ConcurrentRequests_HandleSafely()
    {
        await AddToken();
        // Simulate multiple concurrent requests
        var tasks = new List<Task<HttpResponseMessage>>();
        for (var i = 0; i < 5; i++)
        {
            tasks.Add(HttpClient!.GetAsync(GetRoute));
        }

        // Wait for all requests to complete
        await Task.WhenAll(tasks);

        // Ensure all requests were successful
        foreach (var task in tasks)
        {
            var response = await task;
            response.EnsureSuccessStatusCode();
        }
    }

    [Test]
    public async Task Invoke_CacheKeyVariation_HandleCorrectly()
    {
        await AddToken();
        // Send two GET requests with different parameters to the same endpoint
        var response1 = await HttpClient!.GetAsync(GetRoute);
        response1.EnsureSuccessStatusCode();
        var responseContent1 = await response1.Content.ReadAsStringAsync();

        await AddToken(ValidUserName2, ValidPassword2);
        var response2 = await HttpClient.GetAsync(GetRoute);
        response2.EnsureSuccessStatusCode();
        var responseContent2 = await response2.Content.ReadAsStringAsync();

        // Assert that the responses are different because of different parameters
        Assert.That(responseContent2, Is.Not.EqualTo(responseContent1));
    }

    [Test]
    public async Task Invoke_ErrorHandling_HandleErrorsGracefully()
    {
        try
        {
            await AddToken();
            // Send a GET request to a non-existing endpoint
            var response = await HttpClient!.GetAsync("/api/nonexisting");

            // Ensure that the response status code indicates a not found error (404)
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
