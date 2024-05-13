using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserBlock.Contracts;

namespace UserBlock.IntegrationTest;

public class UserBlockTest
{
    private HttpClient? _client;

    [SetUp]
    public void Setup()
    {
        var factory = new UserBlockApplicationFactory();
        _client = factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
    }

    [Test]
    public async Task BlockUser_AuthenticatedUser_ReturnsOkResult()
    {
        var userInfo = new UserInfo("user1", "password1");
        var jsonContent = JsonContent.Create(userInfo, mediaType: new MediaTypeHeaderValue("application/json"));
        var tokenResult = await _client!.PostAsync("api/authentication/token",
            jsonContent);
        var token = JsonConvert.DeserializeObject<Token>(await tokenResult.Content.ReadAsStringAsync());
        
        // Act
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Value);
        var res = await _client!.PostAsync("api/userBlock/block", jsonContent);

        // Assert
        Assert.That(res, Is.Not.Null);
        Assert.That(res.IsSuccessStatusCode, Is.True);
    }

    // [Test]
    // public async Task UnblockUser_AuthenticatedUser_ReturnsOkResult()
    // {
    //     // Arrange
    //     var request = new HttpRequestMessage(HttpMethod.Post, "/UserBlock/unblock");
    //     request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "your_jwt_token_here");
    //
    //     // Act
    //     var response = await _client!.SendAsync(request);
    //
    //     // Assert
    //     Assert.That(response, Is.Not.Null);
    //     Assert.That(response.IsSuccessStatusCode, Is.True);
    // }
    //
    // [Test]
    // public async Task BlockUser_UnauthenticatedUser_ReturnsUnauthorizedResult()
    // {
    //     // Arrange
    //     var request = new HttpRequestMessage(HttpMethod.Post, "/UserBlock/block");
    //
    //     // Act
    //     var response = await _client!.SendAsync(request);
    //
    //     // Assert
    //     Assert.That(response, Is.Not.Null);
    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    // }
    //
    // [Test]
    // public async Task UnblockUser_UnauthenticatedUser_ReturnsUnauthorizedResult()
    // {
    //     // Arrange
    //     var request = new HttpRequestMessage(HttpMethod.Post, "/UserBlock/unblock");
    //
    //     // Act
    //     var response = await _client!.SendAsync(request);
    //
    //     // Assert
    //     Assert.That(response, Is.Not.Null);
    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    // }
}