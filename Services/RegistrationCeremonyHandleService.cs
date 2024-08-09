using System.Text;
using Microsoft.AspNetCore.DataProtection;
using RelayChat_Identity.Constants;

namespace RelayChat_Identity.Services;

public class RegistrationCeremonyHandleService(IDataProtectionProvider provider)
    : AbstractProtectedCookieStore(provider, DataProtectionPurpose, CookieConstants.RegistrationCeremonyId)
{
    private const string DataProtectionPurpose = "WebAuthn.Net.Demo.RegistrationCeremonyHandle";
    
    public Task SaveAsync(HttpContext httpContext, string registrationCeremonyId)
    {
        Save(httpContext, Encoding.UTF8.GetBytes(registrationCeremonyId));
        return Task.CompletedTask;
    }

    public Task<string?> ReadAsync(HttpContext httpContext)
    {
        if (TryRead(httpContext, out var registrationCeremonyId))
        {
            return Task.FromResult<string?>(Encoding.UTF8.GetString(registrationCeremonyId));
        }

        return Task.FromResult<string?>(null);
    }

    public Task DeleteAsync(HttpContext httpContext)
    {
        Delete(httpContext);
        return Task.CompletedTask;
    }
}
