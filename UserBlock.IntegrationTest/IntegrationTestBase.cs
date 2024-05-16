using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoFixture;
using Newtonsoft.Json;
using UserBlock.Contracts;

namespace UserBlock.IntegrationTest;

public class IntegrationTestBase
{
    protected HttpClient? HttpClient;
    protected readonly string GetRoute = "api/userBlock/GetUser";
    protected readonly string PostRoute = "api/userBlock/BlockUser";
    protected readonly string DeleteRoute = "api/userBlock/UnblockUser";

    protected readonly Fixture AutoFixture = new();
    private const string ValidPassword = "password1";
    private const string ValidUserName = "user1";
    protected const string ValidUserName2 = "user2";
    protected const string ValidPassword2 = "password2";

    [SetUp]
    public void OneTimeSetUp()
    {
        var factory = new UserBlockApplicationFactory("0.1");
        HttpClient = factory.CreateClient();
    }

    [TearDown]
    public void OneTimeTearDown()
    {
        HttpClient?.Dispose();
        HttpClient = null;
    }

    protected async Task<string?> GenerateToken(
        string username = ValidUserName,
        string password = ValidPassword
    )
    {
        var userInfo = new UserRequest(username, password);
        var jsonContent = JsonContent.Create(
            userInfo,
            mediaType: new MediaTypeHeaderValue("application/json")
        );
        var tokenResult = await HttpClient!.PostAsync("api/authentication/token", jsonContent);
        if (!tokenResult.IsSuccessStatusCode)
            return null;

        var token = JsonConvert.DeserializeObject<JwtToken>(
            await tokenResult.Content.ReadAsStringAsync()
        );
        return token.Value;
    }

    protected async Task AddToken(string username = ValidUserName, string password = ValidPassword)
    {
        var token = await GenerateToken(username, password);

        HttpClient!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );
    }
}
