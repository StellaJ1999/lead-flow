using LeadFlow.Application.DTOs.Auth;

namespace LeadFlow.Application.Abstractions.Security;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
}