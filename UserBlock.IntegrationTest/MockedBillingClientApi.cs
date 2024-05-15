using Corp.Billing.Shared;

namespace UserBlock.IntegrationTest;

public class MockedBillingClientApi : IBillingClientApi
{
    public Task<UserFeatureBitMask> GetUserFeaturesAsync()
    {
        // Authorize everything in integration test
        return Task.FromResult(UserFeatureBitMask.BaseFeature);
    }
}