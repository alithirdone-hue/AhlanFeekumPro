using IdentityModel.Client;
using AhlanFeekumPro.Maui.Oidc;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client.Authentication;

namespace AhlanFeekumPro.Maui;

public class MauiRemoteServiceHttpClientAuthenticator : IRemoteServiceHttpClientAuthenticator, ITransientDependency
{
    private readonly ILoginService _loginService;

    public MauiRemoteServiceHttpClientAuthenticator(ILoginService loginService)
    {
        _loginService = loginService;
    }

    public async Task Authenticate(RemoteServiceHttpClientAuthenticateContext context)
    {
        var currentAccessToken = await _loginService.GetAccessTokenAsync();

        if (!currentAccessToken.IsNullOrEmpty())
        {
            context.Request.SetBearerToken(currentAccessToken);
        }
    }
}