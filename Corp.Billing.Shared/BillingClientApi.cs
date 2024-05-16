namespace Corp.Billing.Shared;

public class BillingClientApi(Random random) : IBillingClientApi
{
    public Task<UserFeatureBitMask> GetUserFeaturesAsync()
    {
        var values = Enum.GetValues(typeof(UserFeatureBitMask));
        var randomValue = (UserFeatureBitMask)values.GetValue(random.Next(values.Length))!;
        return Task.FromResult(randomValue);
    }
}
