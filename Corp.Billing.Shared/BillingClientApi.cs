namespace Corp.Billing.Shared;

public class BillingClientApi : IBillingClientApi
{
    public Task<UserFeatureBitMask> GetUserFeaturesAsync()
    {
        // var values = Enum.GetValues(typeof(UserFeatureBitMask));
        // var randomValue = (UserFeatureBitMask)values.GetValue(new Random().Next(values.Length))!;
        // return Task.FromResult(randomValue);
        
        return Task.FromResult(UserFeatureBitMask.LimitedFeature);
    }
}