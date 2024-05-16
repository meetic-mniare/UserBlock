using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using AutoFixture;
using Newtonsoft.Json;
using UserBlock.Contracts;

namespace UserBlock.IntegrationTest;

public class UserBlockControllerTest : IntegrationTestBase
{
    [Test]
    public async Task GetUser_AuthenticatedUser_ReturnsOkResult()
    {
        var token = await GenerateToken();
        Assert.That(token, Is.Not.Null);

        // Act
        HttpClient?.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var res = await HttpClient!.GetAsync(GetRoute);
        var resultUser = await res.Content.ReadFromJsonAsync<UserResponse>();
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
        var userName = AutoFixture.Create<string>();
        var password = AutoFixture.Create<string>();
        var token = await GenerateToken(userName, password);
        Assert.That(token, Is.Null);
        // Act
        HttpClient!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var res = await HttpClient!.GetAsync(GetRoute);
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
        var userName = AutoFixture.Create<string>();
        var password = AutoFixture.Create<string>();
        var token = await GenerateToken(userName, password);
        Assert.That(token, Is.Null);
        // Act
        HttpClient!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var res = await HttpClient.PostAsync(
            PostRoute,
            new StringContent(ValidUserName2, Encoding.UTF8, "application/json")
        );
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
        HttpClient!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var userInfo = new UserRequest(AutoFixture.Create<string>(), null);
        var jsonContent = JsonContent.Create(
            userInfo,
            mediaType: new MediaTypeHeaderValue("application/json")
        );
        var res = await HttpClient.PostAsync(PostRoute, jsonContent);

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
        HttpClient!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var userInfo = new UserRequest(ValidUserName2, null);
        var jsonContent = JsonContent.Create(
            userInfo,
            mediaType: new MediaTypeHeaderValue("application/json")
        );
        var res = await HttpClient.PostAsync(PostRoute, jsonContent);

        var userDto = await res.Content.ReadFromJsonAsync<UserResponse>();

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(res, Is.Not.Null);
            Assert.That(res.IsSuccessStatusCode, Is.True);
            Assert.That(userDto, Is.Not.Null);
            Assert.That(userDto!.Data?.BlockedUsers, Is.Not.Empty);
        });
    }

    [Test]
    // [Ignore("Manual test to ensure context is up to date")]
    public async Task Delete_AuthenticatedUserAndValidUserToBlock_ReturnsOk()
    {
        var token = await GenerateToken();
        Assert.That(token, Is.Not.Null);

        //Verify that user is not blocked
        HttpClient!.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        var testedUserDto = await (
            await HttpClient.GetAsync(GetRoute)
        ).Content.ReadFromJsonAsync<UserResponse>();
        Assert.That(testedUserDto!.Data?.BlockedUsers, Is.Empty);

        // Add blocked user
        var userInfo = new UserRequest(ValidUserName2, null);
        var jsonContent = JsonContent.Create(
            userInfo,
            mediaType: new MediaTypeHeaderValue("application/json")
        );
        var res = await HttpClient.PostAsync(PostRoute, jsonContent);

        var userDto = await res.Content.ReadFromJsonAsync<UserResponse>();
        Assert.That(res, Is.Not.Null);
        Assert.That(res.IsSuccessStatusCode, Is.True);
        Assert.That(userDto, Is.Not.Null);
        Assert.That(userDto.Data?.BlockedUsers, Is.Not.Empty);

        // Delete blocked user
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(DeleteRoute, UriKind.Relative), // Replace "resource" with your resource endpoint
            Content = new StringContent(
                JsonConvert.SerializeObject(userInfo),
                Encoding.UTF8,
                "application/json"
            )
        };
        var response = await HttpClient.SendAsync(request);

        var deleteUserDto = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.That(response, Is.Not.Null);
        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(deleteUserDto, Is.Not.Null);
        Assert.That(deleteUserDto.Data?.BlockedUsers, Is.Empty);
    }
}
