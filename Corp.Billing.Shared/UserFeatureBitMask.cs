namespace Corp.Billing.Shared;

public enum UserFeatureBitMask
{
    None = 0,
    BaseFeature = 1 << 0,
    LimitedFeature = 1 << 1,
}