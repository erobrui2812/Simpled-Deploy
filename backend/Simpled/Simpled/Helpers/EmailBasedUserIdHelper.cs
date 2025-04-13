using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class EmailBasedUserIdHelper : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        var user = connection.User;
        return user?.FindFirst(ClaimTypes.Email)?.Value
            ?? user?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
    }
}
