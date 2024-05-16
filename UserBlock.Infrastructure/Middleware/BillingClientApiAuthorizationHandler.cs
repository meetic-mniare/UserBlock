using System.Security.Claims;
using Corp.Billing.Shared;
using Microsoft.AspNetCore.Authorization;

namespace UserBlock.Infrastructure.Middleware;

public class BillingClientApiAuthorizationHandler(IBillingClientApi billingClientApi)
    : AuthorizationHandler<BillingRequirement>
{
    // Inject the right http client
    // private readonly HttpClient _httpClient = httpClient;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BillingRequirement requirement
    )
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            context.Fail();
            return;
        }

        // Call here the api to get user authorizations
        var response = await billingClientApi.GetUserFeaturesAsync();
        var isAuthorized = response == UserFeatureBitMask.BaseFeature; //
        if (isAuthorized)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
