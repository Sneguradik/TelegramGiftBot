using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Web.ApiServices.Config;

namespace Web.ApiServices.Authorization;

public class TelegramSecretTokenHandler(IOptions<TelegramBotConfig> config) : AuthorizationHandler<TelegramSecretTokenRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TelegramSecretTokenRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var expectedToken = config.Value.Token;
        var receivedToken = httpContext.Request.Headers["X-Telegram-Bot-Api-Secret-Token"].ToString();

        if (!string.IsNullOrEmpty(expectedToken) && expectedToken == receivedToken) context.Succeed(requirement);
        else context.Fail();

        return Task.CompletedTask;
    }
}

public class TelegramSecretTokenRequirement : IAuthorizationRequirement;