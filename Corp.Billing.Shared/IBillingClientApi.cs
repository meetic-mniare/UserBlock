namespace Corp.Billing.Shared;

public interface IBillingClientApi
{
    Task<UserFeatureBitMask> GetUserFeaturesAsync();
}
