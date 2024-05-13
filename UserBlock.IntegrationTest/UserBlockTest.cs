using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using AutoFixture;
using Newtonsoft.Json;
using UserBlock.Contracts;

namespace UserBlock.IntegrationTest;

public class UserBlockTest
{
    private HttpClient? _client;
    private Fixture _fixture;
    private const string ValidPassword = "password1";
    private const string ValidUserName = "user1";
    private const string ValidUserName2 = "user2";

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        var factory = new UserBlockApplicationFactory();
        _client = factory.CreateClient();
        _client?.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    [TearDown]
    public void TearDown()
    {
        if (_client != null)
        {
            _client.Dispose();
            _client = null;
        }
    }

    [Test]
    public async Task GetUser_AuthenticatedUser_ReturnsOkResult()
    {
        var token = await GenerateToken();
        Assert.That(token, Is.Not.Null);

        // Act
        _client?.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var res = await _client!.GetAsync("api/userBlock/GetUser");
        var resultUser = await res.Content.ReadFromJsonAsync<UserDto>();
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(resultUser, Is.Not.Null);
            Assert.That(res, Is.Not.Null);
        });
        Assert.That(res.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task GetUser_NotAuthenticatedUser_ReturnsNotFound()
    {
        var userName = _fixture.Create<string>();
        var password = _fixture.Create<string>();
        var token = await GenerateToken(userName, password);
        Assert.That(token, Is.Null);
        // Act
        _client!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var res = await _client!.GetAsync("api/userBlock/GetUser");
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(res, Is.Not.Null);
            Assert.That(res.IsSuccessStatusCode, Is.False);
            Assert.That(res.ReasonPhrase, Is.EqualTo("Unauthorized"));
        });
    }

    [Test]
    public async Task BlockUser_NotAuthenticatedUser_ReturnsNotFound()
    {
        var userName = _fixture.Create<string>();
        var password = _fixture.Create<string>();
        var token = await GenerateToken(userName, password);
        Assert.That(token, Is.Null);
        // Act
        _client!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var res = await _client.PostAsync("api/userBlock/BlockUser",
            new StringContent(ValidUserName2, Encoding.UTF8, "application/json"));
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(res, Is.Not.Null);
            Assert.That(res.IsSuccessStatusCode, Is.False);
            Assert.That(res.ReasonPhrase, Is.EqualTo("Unauthorized"));
        });
    }

    [Test]
    public async Task BlockUser_AuthenticatedUserButNotValidUserToBlock_ReturnsBadRequest()
    {
        var token = await GenerateToken();
        Assert.That(token, Is.Not.Null);
        // Act
        _client!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var userInfo = new UserInfo(_fixture.Create<string>(), null);
        var jsonContent = JsonContent.Create(userInfo, mediaType: new MediaTypeHeaderValue("application/json"));
        var res = await _client.PostAsync("api/userBlock/BlockUser",
            jsonContent);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(res, Is.Not.Null);
            Assert.That(res.IsSuccessStatusCode, Is.False);
            Assert.That(res.ReasonPhrase, Is.EqualTo("Bad Request"));
        });
    }

    [Test]
    public async Task BlockUser_AuthenticatedUserAndValidUserToBlock_ReturnsOk()
    {
        var token = await GenerateToken();
        Assert.That(token, Is.Not.Null);
        // Act
        _client!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var userInfo = new UserInfo(ValidUserName2, null);
        var jsonContent = JsonContent.Create(userInfo, mediaType: new MediaTypeHeaderValue("application/json"));
        var res = await _client.PostAsync("api/userBlock/BlockUser",
            jsonContent);

        var userDto = await res.Content.ReadFromJsonAsync<UserDto>();

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(res, Is.Not.Null);
            Assert.That(res.IsSuccessStatusCode, Is.True);
            Assert.That(userDto, Is.Not.Null);
            Assert.That(userDto.BlockedUsers, Is.Not.Empty);
        });
    }


    [Test]
    [Ignore("Manual test to ensure context is up to date")]
    public async Task Delete_AuthenticatedUserAndValidUserToBlock_ReturnsOk()
    {
        var token = await GenerateToken();
        Assert.That(token, Is.Not.Null);
        
        //Verify that user is not blocked
        _client!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        var testedUserDto =
            await (await _client.GetAsync("api/userBlock/GetUser")).Content.ReadFromJsonAsync<UserDto>();
        Assert.That(testedUserDto.BlockedUsers, Is.Empty);

        // Add blocked user
        var userInfo = new UserInfo(ValidUserName2, null);
        var jsonContent = JsonContent.Create(userInfo, mediaType: new MediaTypeHeaderValue("application/json"));
        var res = await _client.PostAsync("api/userBlock/BlockUser",
            jsonContent);

        var userDto = await res.Content.ReadFromJsonAsync<UserDto>();
        Assert.That(res, Is.Not.Null);
        Assert.That(res.IsSuccessStatusCode, Is.True);
        Assert.That(userDto, Is.Not.Null);
        Assert.That(userDto.BlockedUsers, Is.Not.Empty);
        
        // Delete blocked user
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("api/userBlock/UnblockUser", UriKind.Relative), // Replace "resource" with your resource endpoint
            Content = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8, "application/json")
        };
        var response = await _client.SendAsync(request);
        
        var deleteUserDto = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.That(response, Is.Not.Null);
        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(deleteUserDto, Is.Not.Null);
        Assert.That(deleteUserDto.BlockedUsers, Is.Empty);
    }

    private async Task<string?> GenerateToken(string username = ValidUserName, string password = ValidPassword)
    {
        var userInfo = new UserInfo(username, password);
        var jsonContent = JsonContent.Create(userInfo, mediaType: new MediaTypeHeaderValue("application/json"));
        var tokenResult = await _client!.PostAsync("api/authentication/token",
            jsonContent);
        if (!tokenResult.IsSuccessStatusCode) return null;

        var token = JsonConvert.DeserializeObject<Token>(await tokenResult.Content.ReadAsStringAsync());
        return token.Value;
    }
}