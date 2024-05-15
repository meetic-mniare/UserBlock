namespace Corp.Billing.Shared;

public class BillingClientApiMock : IBillingClientApi
{
    private string[] _subUsers = ["user1"];

    public Task<UserFeatureBitMask> GetUserFeaturesAsync(string userName, CancellationToken ctx)
    {
        var res = string.IsNullOrEmpty(userName) && _subUsers.Contains(userName)
            ? UserFeatureBitMask.LimitedFeature
            : UserFeatureBitMask.BaseFeature;
        return Task.FromResult(res);
    }
}